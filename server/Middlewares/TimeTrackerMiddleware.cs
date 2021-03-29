using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kafka.Example.Models;
using Kafka.Example.Services;
using Microsoft.AspNetCore.Http;

namespace Kafka.Example.Middlewares
{
    public class TimeTrackerMiddleware
    {
        private readonly ILogService _kafkaLogService;
        private readonly ILogService _fileLogService;
        private readonly RequestDelegate _next;
        private readonly string[] toBeTrackedPaths = { "/api/products", "/api/products/" };

        public TimeTrackerMiddleware(RequestDelegate next, KafkaLogService kafkaLogService, FileLogService fileLogService)
        {
            _next = next;
            _kafkaLogService = kafkaLogService;
            _fileLogService = fileLogService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var requestedPath = httpContext.Request.Path.ToString().ToLower();
            if (toBeTrackedPaths.Any(paths => paths == requestedPath))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                await _next(httpContext);
                stopwatch.Stop();

                if (httpContext.Response.StatusCode != 405)
                {
                    var responseLog = new ResponseLog(httpContext.Request.Method, stopwatch.ElapsedMilliseconds);
                    _fileLogService.SendAsync(responseLog);
                    _kafkaLogService.SendAsync(responseLog);
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
    }
}