using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kafka.Example.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Kafka.Example.Controllers
{
    [ApiController]
    [Route("[Controller]/api/products/")]
    public class HealthController : ControllerBase
    {
        private readonly List<ResponseLog> logs = new List<ResponseLog>();
        private readonly NpgsqlConnection npgsqlConnection;

        public HealthController(IConfiguration configuration) => 
        npgsqlConnection = new NpgsqlConnection(configuration["Database:ConnectionString"]);

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await npgsqlConnection.OpenAsync();
            var oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();

            await using (var cmd = new NpgsqlCommand($"select * from net_logs where timestamputc >= {oneHourAgo} order by timestamputc", npgsqlConnection))
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