using Confluent.Kafka;
using Kartaca.Intern.Models;

namespace Kartaca.Intern.Services
{
    public class KafkaLogService
    {
        // todo: get configurations from environments or another
        private const string topic = "response_log";
        private const string server = "kartaca-kafka:9092";
        private readonly ProducerConfig producerConfig = new ProducerConfig()
        { 
            BootstrapServers = server,
            MessageTimeoutMs = 500,
            Acks = Acks.None // i think we don't need to care all that data, we just need to speed to catch them
        };

        public async void SendAsync(ResponseLog responseLog)
        {
            using (var producer = new ProducerBuilder<string, string>(producerConfig)
            .SetValueSerializer(Serializers.Utf8)
            .Build())
            {
                var message = new Message<string, string> { Value = responseLog.Message };
                producer.ProduceAsync(topic, message);
            }
        }
    }
}