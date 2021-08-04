using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
namespace Kafka.Example.Services
{
    public class FileLogService : LogService
    {
        private readonly string logFilePath = "./";

        public FileLogService(ILogger<FileLogService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            logFilePath += topic;
            createLogFileIfNotExist();
        }

        private void createLogFileIfNotExist()
        {
            if (!File.Exists(logFilePath))
            {
                _logger.LogWarning("Log file isn't found.");
                File.Create(logFilePath).DisposeAsync();
                _logger.LogInformation($"New log file created: {Path.GetFullPath(logFilePath)}");
            }
        }

        public override void SendAsync(ResponseLog responseLog)
        {
            using (var writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLineAsync(responseLog.Message);
            }
            _logger.LogInformation($"{responseLog.Message} has been written to {logFilePath} file.");
        }
    }
}