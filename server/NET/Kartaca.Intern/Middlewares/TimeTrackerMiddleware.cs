using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kartaca.Intern.Middlewares
{
    public class TimeTrackerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /*  private readonly Stopwatch stopwatch = new Stopwatch();
            It doesn't work compatible with requests when do global definition because of the using in the async area. 
            If every request use same Stopwatch object, a conflict will definitly be there. 
            While a request's starting the watch another request try to stop it and take elapsedTime, 
            this is the just one of the conflicts.
        */

        public TimeTrackerMiddleware(ILogger<TimeTrackerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            _logger.LogInformation($"Start {httpContext.Request.Method} : {httpContext.Request.Path}");
            // todo: kafka

            await _next(httpContext);

            stopwatch.Stop();
            _logger.LogInformation($"End {httpContext.Request.Method} : {httpContext.Request.Path} in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}