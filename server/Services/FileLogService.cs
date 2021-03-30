using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
namespace Kafka.Example.Services
{
    public class FileLogService : ILogService
    {
        private readonly string logFilePath = "./";
        private readonly ILogger _logger;

        public FileLogService(ILogger<FileLogService> logger, IConfiguration configuration)
        {
            _logger = logger;
            logFilePath += configuration["Kafka:Topic"];
            checkAndCreateLogFileIfNotExist();
        }

        private void checkAndCreateLogFileIfNotExist()
        {
            if (!File.Exists(logFilePath))
            {
                _logger.LogWarning("Log file isn't found.");
                File.Create(logFilePath).DisposeAsync();
                _logger.LogInformation($"New log file created: {Path.GetFullPath(logFilePath)}");
            }
        }

        public void SendAsync(ResponseLog responseLog)
        {
            using (var writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLineAsync(responseLog.Message);
            }
            _logger.LogInformation($"{responseLog.Message} has been written to {logFilePath} file.");
        }
    }
}