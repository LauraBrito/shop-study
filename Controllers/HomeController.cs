using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            return Ok(new
            {
                message = "OK"
            });
        }
    }
}