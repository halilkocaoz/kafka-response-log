using System.Threading.Tasks;
using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kafka.Example.Services
{
    public abstract class LogService
    {
        protected readonly ILogger _logger;
        protected readonly string topic;

        protected LogService(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            topic = configuration["Kafka:Topic"];
        }

        public abstract Task SendAsync(ResponseLog responseLog);
    }
}