using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
//* note: this work also could be implement as action filter
namespace Kartaca.Intern.Middlewares
{
    public class DelayerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Random _random = new Random();
        public DelayerMiddleware(RequestDelegate next) => _next = next;

        private int delayMS;
        private const int maxDelayMS = 3000;
        public async Task InvokeAsync(HttpContext httpContext)
        {
            delayMS = _random.Next(maxDelayMS);
            System.Threading.Thread.Sleep(delayMS);
            await _next(httpContext);
        }
    }
}