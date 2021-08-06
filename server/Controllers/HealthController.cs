using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kafka.Example.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Kafka.Example.Controllers
{
    [ApiController]
    [Route("[Controller]/api/")]
    public class HealthController : ControllerBase
    {
        private readonly List<ResponseLog> logs = new List<ResponseLog>();
        private readonly NpgsqlConnection npgsqlConnection;

        public HealthController(IConfiguration configuration) => npgsqlConnection = new NpgsqlConnection(configuration["Database:ConnectionString"]);

        [HttpGet("products")]
        public async Task<IActionResult> ProductsHealth()
        {
            await npgsqlConnection.OpenAsync();
            var oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds();

            await using (var cmd = new NpgsqlCommand(
                $"SELECT * FROM net_logs WHERE timestamp >= {oneHourAgo} ORDER BY timestamp", npgsqlConnection))
            await using (var reader = await cmd.ExecuteReaderAsync()) while (await reader.ReadAsync())
            {
                logs.Add(new ResponseLog(reader.GetString(0), reader.GetInt64(1), reader.GetInt64(2)));
            }

            #pragma warning disable CS4014
            npgsqlConnection.CloseAsync();

            return logs != null && logs.Any() ? Ok(logs) : NoContent();
        }
    }
}