using System;
using Kafka.Example.Models;
using Kafka.Example.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Kafka.Example.Filters
{
    public class TimeTracker : ActionFilterAttribute
    {
        private readonly ILogService _kafkaLogService;
        private readonly ILogService _fileLogService;

        public TimeTracker(ILoggerFactory loggerFactory)
        {
            _kafkaLogService = new KafkaLogService(loggerFactory.CreateLogger<KafkaLogService>());
            _fileLogService = new FileLogService(loggerFactory.CreateLogger<FileLogService>());
        }

        public override void OnActionExecuting(ActionExecutingContext context) =>
        context.RouteData.Values["timeBeforeActionExecute"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode != 405)
            {
                var nowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var elapsedTime = nowTime - long.Parse(context.RouteData.Values["timeBeforeActionExecute"].ToString());
                
                var responseLog = new ResponseLog(context.HttpContext.Request.Method, elapsedTime);
                _fileLogService.SendAsync(responseLog);
                _kafkaLogService.SendAsync(responseLog);
            }
        }
    }
}