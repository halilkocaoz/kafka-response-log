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

        /*  private readonly Stopwatch stopwatch = new Stopwatch();
            It doesn't work compatible with requests when do global definition because of the using in the async area. 
            If every request use same Stopwatch object, a conflict will definitly be there. 
            While a request's starting the watch another request try to stop it and take elapsedTime, 
            this is the just one of the conflicts.
        */

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

            /* 
                if the response 404, don't message it because that request could be anything
                when a api consumer try to get result from path which isn't exist 
                if we send this response time to kafka, it break accuracy.
            */
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