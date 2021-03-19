using Confluent.Kafka;
using Kartaca.Intern.Models;

namespace Kartaca.Intern.Services
{
    public class KafkaLogService
    {
        // todo: get configurations from environments or another
        private const string topic = "response_log";
        private const string server = "kartaca-kafka-net:9092";

        private readonly ProducerConfig producerConfig = new ProducerConfig()
        { // todo: research optimum configs.
            MessageMaxBytes = 3000000,
            BootstrapServers = server,
            MessageTimeoutMs = 1000
        };

        public async void SendAsync(ResponseLog responseLog)
        {
            // todo message create with ctor
            var messageValue = $"{responseLog.Path} {responseLog.Method} {responseLog.ElapsedTime} {responseLog.Timestamp}";
            using (var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetValueSerializer(Serializers.Utf8)
            .Build())
            {
                var message = new Message<string, string> { Value = messageValue };
                producer.ProduceAsync(topic, message);
            }
        }
    }
}