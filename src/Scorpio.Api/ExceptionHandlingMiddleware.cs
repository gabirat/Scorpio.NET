using Matty.Framework;
using Matty.Framework.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Scorpio.Api
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, httpContext, HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(Exception ex, HttpContext context, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var correlationId = Guid.NewGuid();
            _logger.LogError(ex, "Global exception logger, correlationId: " + correlationId);

            var uiMessage = $"{ex.Message} {ex.InnerException?.Message}";

            var response = new ServiceResult<object> { Metadata = { ProblemDetails = CreateProblemDetails(ex, context, correlationId) } };
            response.Alerts.Add(new Alert(uiMessage, MessageType.Exception));

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private static ProblemDetails CreateProblemDetails(Exception ex, HttpContext context, Guid correlationId)
        {
            var details = new ProblemDetails
            {
                Title = "Unhandled exception",
                Type = "Exception",
                Detail = ex.Message,
                Status = context.Response.StatusCode,
                Instance = correlationId.ToString()
            };

            details.Extensions.Add("timestamp", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            details.Extensions.Add("location", context.Request.Path);
            return details;
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static void UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
