using System;
using System.Threading.Tasks;
using Kafka.Example.Models;
using Kafka.Example.Services;
using Microsoft.AspNetCore.Http;
using Kafka.Example.Filters;

namespace Kafka.Example.Middlewares
{
    public class ResponseLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, KafkaLogService kafkaLogService, FileLogService fileLogService)
        {
            await _next(httpContext);
            
            /// <see cref="TimeTracker"/>
            
            var timeBeforeProcessEndpoint = httpContext.Items["timeBeforeProcessEndpoint"] != null
            ? (long)httpContext.Items["timeBeforeProcessEndpoint"]
            : default;

            if (timeBeforeProcessEndpoint != default)
            {
                var timeAfterProcessEndpoint = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var elapsedTime = timeAfterProcessEndpoint - timeBeforeProcessEndpoint;

                var responseLog = new ResponseLog(httpContext.Request.Method, elapsedTime);

                #pragma warning disable CS4014
                kafkaLogService.SendAsync(responseLog);
                fileLogService.SendAsync(responseLog);
            }
        }
    }
}