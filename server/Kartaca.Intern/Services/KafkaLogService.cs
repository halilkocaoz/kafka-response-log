using Confluent.Kafka;
using Kartaca.Intern.Models;
using Microsoft.Extensions.Logging;

namespace Kartaca.Intern.Services
{
    public class KafkaLogService : ILogService
    {
        private readonly ILogger _logger;
        private const string topic = "response_log";
        private const string server = "kafka:9092";
        private readonly ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = server,
            MessageTimeoutMs = 1000,
            Acks = Acks.None
        };

        public KafkaLogService(ILogger<KafkaLogService> logger) => _logger = logger;

        public async void SendAsync(ResponseLog responseLog)
        {
            var message = new Message<string, string> { Value = responseLog.Message };
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