using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Context;
using AuthenticationApi.Models;
using AuthenticationApi.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Register user
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            user.password = HashFunctions.HashPassword(user.password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generar token con duración de una hora
            var tokenString = TokenFunctions.GenerateToken(user.id);

            // Devolver token en la respuesta
            return CreatedAtAction("GetUser", new { id = user.id }, new { user.id, Token = tokenString });
        }

        //POST Loggin user
        [HttpPost("LoginUser")]
        public async Task<ActionResult> LoginUser(LoginUser loginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == loginModel.email);
            if (user == null)
            {
                return BadRequest("Correo no registrado");
            }
            if (user == null || !HashFunctions.VerifyPassword(loginModel.password, user.password))
            {
                BadRequest("Contraseña incorrectos");
            }

            // Generar token con duración de una hora
            var tokenString = TokenFunctions.GenerateToken(user.id);

            // Devolver token y la información del usuario en la respuesta
            return Ok(new { user.id, Token = tokenString });
        }

        [HttpGet("GetUser/{token}")]
        public async Task<ActionResult<User>> GetUser(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token no válido");
            }
            int userId;
            Console.WriteLine("this is rodo");
            var isValidToken = TokenFunctions.ValidateToken(token, out userId);
            Console.WriteLine(""+userId+"", isValidToken);
            if (isValidToken)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Usuario no encontrado");
                }
                user.password = "";
                return Ok(user);
            }

            return Unauthorized();
        }
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }
    }
}
