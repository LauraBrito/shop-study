using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products
                                        .Include(x => x.Category)
                                        .AsNoTracking()
                                        .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int categoryId, [FromServices] DataContext context)
        {
            var product = await context.Products
                                        .Include(x => x.Category)
                                        .AsNoTracking()
                                        .Where(x => x.CategoryId == categoryId)
                                        .FirstOrDefaultAsync();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromBody] Product model,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o produto" });
            }

        }
    }

}