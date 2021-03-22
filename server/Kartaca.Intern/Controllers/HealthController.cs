using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kartaca.Intern.Controllers
{
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("health/api/products/")]
        public async Task<IActionResult> Get() => Ok();
    }
}