using Kartaca.Intern.Models;
using Microsoft.Extensions.Logging;
using System.IO;
namespace Kartaca.Intern.Services
{
    public class FileLogService : ILogService
    {
        private readonly string logFilePath = "./response_log";
        private readonly ILogger _logger;

        public FileLogService(ILogger<FileLogService> logger)
        {
            _logger = logger;
            checkAndCreateLogFileIfNotExist();
        }

        private void checkAndCreateLogFileIfNotExist()
        {
            if (File.Exists(logFilePath) is false)
            {
                _logger.LogWarning("Log file isn't found.");
                File.Create("./response_log").DisposeAsync();
                _logger.LogInformation($"New log file created: {Path.GetFullPath(logFilePath)}");
            }
        }

        public void SendAsync(ResponseLog responseLog)
        {
            using (var writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLineAsync(responseLog.Message);
            }
            _logger.LogInformation($"{responseLog.Message} has been written {logFilePath} file.");
        }
    }
}