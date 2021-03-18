using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kartaca.Intern.Middlewares
{
    public class DelayerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly Random _random = new Random();

        public DelayerMiddleware(ILogger<DelayerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }
        
        private int delayMS;
        private const int maxDelayMS = 3000;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            delayMS = _random.Next(maxDelayMS);
            _logger.LogInformation($"Will delay {delayMS} ms");
            System.Threading.Thread.Sleep(delayMS);
            await _next(httpContext);
        }
    }
}