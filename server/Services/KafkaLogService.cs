using Confluent.Kafka;
using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kafka.Example.Services
{
    public class KafkaLogService : ILogService
    {
        private readonly ILogger _logger;
        private readonly string topic;
        private const string server = "kafka:9092";
        private readonly ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = server,
            MessageTimeoutMs = 1000,
            Acks = Acks.None
        };

        public KafkaLogService(ILogger<KafkaLogService> logger, IConfiguration configuration)
        {
            topic = configuration["Kafka:Topic"];
            _logger = logger;
        }
        
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