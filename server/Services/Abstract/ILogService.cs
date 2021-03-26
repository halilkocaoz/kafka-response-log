using Kafka.Example.Models;

namespace Kafka.Example.Services
{
    public interface ILogService
    {
        void SendAsync(ResponseLog responseLog);
    }
}