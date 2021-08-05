using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kafka.Example.Filters
{
    public class TimeTracker : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context) =>
        context.HttpContext.Items["timeBeforeProcessEndpoint"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}