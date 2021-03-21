using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kartaca.Intern.Filters
{
    public class Delayer : ActionFilterAttribute
    {
        private readonly int _maxDelayMS;
        public Delayer(int maxDelayMS) => _maxDelayMS = maxDelayMS;
        private readonly Random _random = new Random();

        public override void OnActionExecuting(ActionExecutingContext context) => System.Threading.Thread.Sleep(_random.Next(_maxDelayMS));
    }
}