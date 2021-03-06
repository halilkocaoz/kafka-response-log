namespace Kafka.Example.Models
{
    public class ResponseLog
    {
        public ResponseLog(string method, long elapsedTime, long timestamp)
        {
            Method = method;
            ElapsedTime = elapsedTime;
            Timestamp = timestamp;
        }

        public string Method { get; set; }
        public long Timestamp { get; set; }
        public long ElapsedTime { get; set; }
    }
}