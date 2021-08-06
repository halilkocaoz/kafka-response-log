using Kafka.Example.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

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
        
        public override async Task SendAsync(string responseLogMessage)
        {
            using (var writer = new StreamWriter(logFilePath, true))
            {
                await writer.WriteLineAsync(responseLogMessage);
            }
            _logger.LogInformation($"{responseLogMessage} has been written to {logFilePath} file.");
        }
    }
}