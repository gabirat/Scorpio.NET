using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Scorpio.Api.DataAccess;
using Scorpio.Api.EventHandlers;
using Scorpio.Api.Events;
using Scorpio.Api.Hubs;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.RabbitMQ;

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

            // This allows access http context and user in constructor
            services.AddHttpContextAccessor();

            // Register strongly typed config mapping
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));
            services.Configure<MongoDbConfiguration>(Configuration.GetSection("MongoDb"));

            // Register event bus
            services.AddRabbitMqConnection(Configuration);
            services.AddRabbitMqEventBus(Configuration);

            // TODO: automatically register via assembly scanning
            services.AddTransient<UpdateRoverPositionEventHandler>();
            services.AddTransient<TestEventHandler>();

            // Repositories
            services.AddTransient<IUiConfigurationRepository, UiConfigurationRepository>();

            var dupa = Configuration["BACKEND_ORIGIN"];
            var url = "http://" + (dupa ?? "localhost:3000");
            services.AddCors(settings =>
            {
                settings.AddPolicy("corsPolicy", builder =>
                {
                    builder.WithOrigins(url)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandlingMiddleware();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            if (env.EnvironmentName.ToLower() == "development")
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("corsPolicy");
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MainHub>("/hub/main");
            });

            UseEventBus(app);
        }

        private static void UseEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UpdateRoverPositionEvent, UpdateRoverPositionEventHandler>();
            eventBus.Subscribe<Test, TestEventHandler>();
        }
    }
}
