
using System;

namespace Kartaca.Intern.Models
{
    public class ResponseLog
    {
        public ResponseLog(string method, string path, long elapsedTime)
        {
            Method = method;
            Path = path;
            ElapsedTime = elapsedTime;
            Message = $"{Method} {ElapsedTime} {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public long ElapsedTime { get; set; }
        public string Message { get; set; }
    }
}