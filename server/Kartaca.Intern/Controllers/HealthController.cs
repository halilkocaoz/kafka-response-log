using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kartaca.Intern.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Kartaca.Intern.Controllers
{
    [ApiController]
    [Route("[Controller]/api/products/")]
    public class HealthController : ControllerBase
    {
        private List<ResponseLog> logs = new List<ResponseLog>();
        private const string conStr = "Host=kartaca-postgres;Username=postgres;Password=psqlpass;Database=postgres";
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //todo: refactor
            var oneHourAgoFromNowAsUnixSeconds = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();
            var query = $"select * from net_logs where timestamputc >= {oneHourAgoFromNowAsUnixSeconds}";
            
            await using var npgsqlConnection = new NpgsqlConnection(conStr);
            await npgsqlConnection.OpenAsync();

            await using (var cmd = new NpgsqlCommand(query, npgsqlConnection))
            await using (var reader = await cmd.ExecuteReaderAsync()) while (await reader.ReadAsync())
                {
                    logs.Add(new ResponseLog(reader.GetString(0), reader.GetInt64(1), reader.GetInt64(2)));
                }
                
            #pragma warning disable CS4014
            npgsqlConnection.CloseAsync();

            return logs?.Count > 0 ? Ok(logs) : NoContent();
        }
    }
}