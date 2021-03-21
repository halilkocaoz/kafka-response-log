using System.Diagnostics;
using System.Threading.Tasks;
using Kartaca.Intern.Models;
using Kartaca.Intern.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kartaca.Intern.Middlewares
{
    public class TimeTrackerMiddleware
    {
        private readonly KafkaLogService _kafkaLogService;
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public TimeTrackerMiddleware(ILogger<TimeTrackerMiddleware> logger, RequestDelegate next, KafkaLogService kafkaLogService)
        {
            _logger = logger;
            _next = next;
            _kafkaLogService = kafkaLogService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _next(httpContext);
            stopwatch.Stop();

            if (httpContext.Response.StatusCode != 404)
            {
                _kafkaLogService.SendAsync(
                    new ResponseLog(httpContext.Request.Method,
                    httpContext.Request.Path,
                    stopwatch.ElapsedMilliseconds));
            }
        }
    }
}