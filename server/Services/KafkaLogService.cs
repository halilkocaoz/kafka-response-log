using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kafka.Example.Services
{
    public class KafkaLogService : LogService
    {
        private readonly ProducerConfig producerConfig;

        public KafkaLogService(ILogger<KafkaLogService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            producerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["kafka:server"],
                MessageTimeoutMs = 1000,
                Acks = Acks.None
            };
        }

        public override async Task SendAsync(string responseLogMessage)
        {
            Message<string, string> message = new Message<string, string> { Value = responseLogMessage };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).SetValueSerializer(Serializers.Utf8).Build())
            {
                try
                {
                    await producer.ProduceAsync(topic, message);
                    _logger.LogInformation($"{responseLogMessage} has been sent to {topic} topic.");
                }
                catch (KafkaException e)
                {
                    _logger.LogWarning(e.Error.Reason);
                }
            }
        }
    }
}