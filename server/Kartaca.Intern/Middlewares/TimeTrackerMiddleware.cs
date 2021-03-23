using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kartaca.Intern.Models;
using Kartaca.Intern.Services;
using Microsoft.AspNetCore.Http;

namespace Kartaca.Intern.Middlewares
{
    public class TimeTrackerMiddleware
    {
        private readonly KafkaLogService _kafkaLogService;
        private readonly RequestDelegate _next;
        public TimeTrackerMiddleware(RequestDelegate next, KafkaLogService kafkaLogService)
        {
            _next = next;
            _kafkaLogService = kafkaLogService;
        }
        private readonly string[] toBeTrackedPaths = { "/api/products", "/api/products/" };

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var requestedPath = httpContext.Request.Path.ToString().ToLower();
            if (toBeTrackedPaths.Any(paths => paths == requestedPath))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                await _next(httpContext);
                stopwatch.Stop();

                if (httpContext.Response.StatusCode != 405 && httpContext.Response.StatusCode != 404)
                    _kafkaLogService.SendAsync(
                        new ResponseLog(httpContext.Request.Method,
                        stopwatch.ElapsedMilliseconds));
            }
            else
                await _next(httpContext);
        }
    }
}