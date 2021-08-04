using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kafka.Example.Filters
{
    public class Delayer : ActionFilterAttribute
    {
        private readonly int maxDelayMs;
        private readonly Random random;
        
        public Delayer(int maxDelayMS)
        {
            maxDelayMs = maxDelayMS;
            random = new Random();
        }

        public override void OnActionExecuting(ActionExecutingContext context) => System.Threading.Thread.Sleep(random.Next(maxDelayMs));
    }
}