using System;
using System.Text.Json.Serialization;

namespace Kafka.Example.Models
{
    public class ResponseLog
    {
        public ResponseLog(string method, long elapsedTime)
        {
            Method = method;
            ElapsedTime = elapsedTime;
            Message = $"{Method} {ElapsedTime} {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}".ToUpper();
        }
        public ResponseLog(string method, long elapsedTime, long timestamp)
        {
            Method = method;
            ElapsedTime = elapsedTime;
            Timestamp = timestamp;
        }

        public string Method { get; set; }
        public long Timestamp { get; set; }
        public long ElapsedTime { get; set; }

        [JsonIgnore]
        public string Message { get; private set; }
    }
}