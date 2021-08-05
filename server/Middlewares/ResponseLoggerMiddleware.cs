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
        private long _timeBeforeActionExecute;

        public ResponseLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, KafkaLogService kafkaLogService, FileLogService fileLogService)
        {
            await _next(httpContext);
            /// <see cref="TimeTracker"/>
            _timeBeforeActionExecute = httpContext.Items["timeBeforeActionExecute"] != null
            ? (long)httpContext.Items["timeBeforeActionExecute"]
            : default;

            if (_timeBeforeActionExecute != default)
            {
                var timeAfterProcessEndpoint = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var elapsedTime = timeAfterProcessEndpoint - _timeBeforeActionExecute;

                var responseLog = new ResponseLog(httpContext.Request.Method, elapsedTime);

                #pragma warning disable CS4014
                kafkaLogService.SendAsync(responseLog);
                fileLogService.SendAsync(responseLog);
            }
        }
    }
}