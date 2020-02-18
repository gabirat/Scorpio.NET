using Matty.Framework;
using Matty.Framework.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Scorpio.Api.DataAccess;
using Scorpio.Api.EventHandlers;
using Scorpio.Api.Events;
using Scorpio.Api.HostedServices;
using Scorpio.Api.Hubs;
using Scorpio.Gamepad.Processors;
using Scorpio.Gamepad.Processors.Mixing;
using Scorpio.Instrumentation.Ubiquiti;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using Scorpio.Messaging.RabbitMQ;
using Scorpio.Messaging.Sockets;
using Scorpio.ProcessRunner;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Scorpio.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Use NewtonSoft JSON as default serializer
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new ServiceResult<object>(context.ModelState
                        .Where(x => !string.IsNullOrEmpty(x.Value.Errors.FirstOrDefault()?.ErrorMessage))
                        .Select(x => new Alert(x.Value.Errors.FirstOrDefault()?.ErrorMessage, MessageType.Error))
                        .ToList());

                    return new BadRequestObjectResult(result);
                };
            });

            // SignalR - real time messaging with front end
            services.AddSignalR(settings =>
            {
                settings.EnableDetailedErrors = true;
            })
            .AddMessagePackProtocol();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ScorpioAPI", Version = "v1" });
            });

            services.AddLogging(opt => { opt.AddConsole(c => { c.TimestampFormat = "[HH:mm:ss:fff] "; }); });

            // This allows access http context and user in constructor
            services.AddHttpContextAccessor()
                .Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"))
                .Configure<MongoDbConfiguration>(Configuration.GetSection("MongoDb"))
                .Configure<UbiquitiPollerConfiguration>(Configuration.GetSection("Ubiquiti"))
                .Configure<SocketConfiguration>(Configuration.GetSection("socketClient"))
                //.AddRabbitMqConnection(Configuration)
                //.AddRabbitMqEventBus(Configuration)
                .AddSocketClientConnection()
                .AddSocketClientEventBus()
                .AddTransient<IGenericProcessRunner, GenericProcessRunner>()
                .AddTransient<SaveSensorDataEventHandler>()
                .AddTransient<SaveManySensorDataEventHandler>()
                .AddTransient<RoverControlEventHandler>()
                .AddTransient<UbiquitiDataReceivedEventHandler>()
                .AddTransient<IUiConfigurationRepository, UiConfigurationRepository>()
                .AddTransient<ISensorRepository, SensorRepository>()
                .AddTransient<ISensorDataRepository, SensorDataRepository>()
                .AddTransient<IStreamRepository, StreamRepository>()
                .AddTransient<UbiquitiStatsProvider>()
                .AddTransient<IGamepadProcessor<RoverMixer, RoverProcessorResult>, ExponentialGamepadProcessor<RoverMixer, RoverProcessorResult>>()
                .AddUbiquitiPoller(Configuration)
                .AddCorsSetup(Configuration)
                .AddHealthChecks(Configuration);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.ToLower() == "development")
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("corsPolicy");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseRouting();

            app.UseExceptionHandlingMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MainHub>("/hub");
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = WriteHealthResponse
                });
            });

            UseEventBus(app);
        }

        private static IApplicationBuilder UseEventBus(IApplicationBuilder app)
        {
            // RabbitMQ connection requires connecting - this can be refactored later
            var rabbitMqConnection = app.ApplicationServices.GetService<IRabbitMqConnection>();
            rabbitMqConnection?.TryConnect();

            var eventBus = app.ApplicationServices.GetService<IEventBus>();
            eventBus.Subscribe<SaveSensorDataEvent, SaveSensorDataEventHandler>();
            eventBus.Subscribe<SaveManySensorDataEvent, SaveManySensorDataEventHandler>();
            eventBus.Subscribe<UbiquitiDataReceivedEvent, UbiquitiDataReceivedEventHandler>();

            return app;
        }

        private static Task WriteHealthResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("isHealthy", result.Status == HealthStatus.Healthy),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("isHealthy", pair.Value.Status == HealthStatus.Healthy),
                        new JProperty("description", pair.Value.Description)))))));

            return context.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }

    public static class StartupExtensions
    {
        public static IServiceCollection AddUbiquitiPoller(this IServiceCollection services, IConfiguration config)
        {
            var enabled = config.GetValue<bool>("Ubiquiti:EnablePoller");

            if (enabled)
            {
                services.AddHostedService<UbiquitiPollerHostedService>();
            }

            return services;
        }

        public static IServiceCollection AddCorsSetup(this IServiceCollection services, IConfiguration config)
        {
            var corsOrigins = "http://" + (config["BACKEND_ORIGIN"] ?? "localhost:3000");
            services.AddCors(settings =>
            {
                settings.AddPolicy("corsPolicy", builder =>
                {
                    builder.WithOrigins(corsOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static void AddHealthChecks(this IServiceCollection services, IConfiguration config)
        {
            var user = config["RabbitMq:userName"];
            var password = config["RabbitMq:password"];
            var host = config["RabbitMq:host"];
            var port = config["RabbitMq:port"];
            var virtualHost = config["RabbitMq:virtualHost"];

            var rabbitMqConnectionString = $"amqp://{user}:{password}@{host}:{port}{virtualHost}";
            var mongoDbConnectionString = config.GetValue<string>("MongoDb:ConnectionString");

            var timeout = TimeSpan.FromSeconds(1.5);

            services.AddHealthChecks()
                //.AddRabbitMQ(rabbitMqConnectionString, sslOption: null, name: "RabbitMQ", timeout: timeout)
                .AddMongoDb(mongoDbConnectionString, timeout: timeout, name: "MongoDb");
        }
    }
}
