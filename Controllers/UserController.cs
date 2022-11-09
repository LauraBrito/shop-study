using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {

        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            return await context.Users.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Enroll([FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";
                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";
                return model;
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(int id,
            [FromBody] User model,
           [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Essa categoria já foi atualizada" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices] DataContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await context.Users
                                    .AsNoTracking()
                                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                                    .FirstOrDefaultAsync();

            if (user == null) return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpGet]
        [Route("anonimo")]
        [AllowAnonymous]
        public string Anonimo() => "Anonimo";


        [HttpGet]
        [Route("autenticado")]
        [Authorize]
        public string Autenticado() => "Autenticado";

        [HttpGet]
        [Route("funcionario")]
        [Authorize(Roles = "employee")]
        public string Funcionario() => "Funcionario";

        [HttpGet]
        [Route("gerente")]
        [Authorize(Roles = "manager")]
        public string Gerente() => "Gerente";
    }
}