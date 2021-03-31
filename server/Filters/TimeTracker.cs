using System;
using Kafka.Example.Models;
using Kafka.Example.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kafka.Example.Filters
{
    public class TimeTracker : ActionFilterAttribute
    {
        private readonly KafkaLogService _kafkaLogService;
        private readonly FileLogService _fileLogService;

        public TimeTracker(KafkaLogService kafkaLogService, FileLogService fileLogService)
        {
            _kafkaLogService = kafkaLogService;
            _fileLogService = fileLogService;
        }

        public override void OnActionExecuting(ActionExecutingContext context) =>
        context.RouteData.Values["timeBeforeActionExecute"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var timeAfterProcessEndpoint = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (context.HttpContext.Response.StatusCode != 405)
            {
                var elapsedTime = timeAfterProcessEndpoint - long.Parse(context.RouteData.Values["timeBeforeActionExecute"].ToString());

                var responseLog = new ResponseLog(context.HttpContext.Request.Method, elapsedTime);
                _fileLogService.SendAsync(responseLog);
                _kafkaLogService.SendAsync(responseLog);
            }
        }
    }
}