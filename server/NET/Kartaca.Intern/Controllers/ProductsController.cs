using Microsoft.AspNetCore.Mvc;

namespace Kartaca.Intern.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => NoContent();

        [HttpPost]
        public IActionResult Post() => NoContent();

        [HttpPut]
        public IActionResult Put() => NoContent();

        [HttpDelete]
        public IActionResult Delete() => NoContent();
    }
}