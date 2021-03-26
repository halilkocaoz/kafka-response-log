using Kartaca.Intern.Models;

namespace Kartaca.Intern.Services
{
    public interface ILogService
    {
        void SendAsync(ResponseLog responseLog);
    }
}