using Kartaca.Intern.Models;
using Microsoft.Extensions.Logging;

namespace Kartaca.Intern.Services
{
    public class FileLogService : ILogService
    {
        private readonly ILogger _logger;

        public FileLogService(ILogger<FileLogService> logger) => _logger = logger;

        public void SendAsync(ResponseLog responseLog)
        {
            _logger.LogInformation(responseLog.Message);
        }
    }
}