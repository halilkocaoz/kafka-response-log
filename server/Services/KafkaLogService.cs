using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kafka.Example.Services
{
    public class KafkaLogService : LogService
    {
        private readonly static ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = "kafka:9092",
            MessageTimeoutMs = 1000,
            Acks = Acks.None
        };
        private static Message<string, string> message;

        public KafkaLogService(ILogger<KafkaLogService> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public override async Task SendAsync(ResponseLog responseLog)
        {
            message = new Message<string, string> { Value = responseLog.Message };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).SetValueSerializer(Serializers.Utf8).Build())
            {
                try
                {
                    await producer.ProduceAsync(topic, message);
                    _logger.LogInformation($"{responseLog.Message} has been sent to {topic} topic.");
                }
                catch (KafkaException e)
                {
                    _logger.LogWarning(e.Error.Reason);
                }
            }
        }
    }
}