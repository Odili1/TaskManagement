using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;

        public UserController(TaskManagementDbContext context)
        {
            _context = context;
        }

        // Create Operation
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // Fetch By Id Operation
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            // Check if user is exist in the db
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }



        // Fetch All Operation
        [HttpGet]
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // Update Operation
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            try
            {
                // check if the user id exist
                var userExists = await _context.Users.FindAsync(id);

                if (userExists == null)
                {
                    return NotFound();
                }


                //_context.Entry(user).State = EntityState.Modified;    

                userExists.Email = user.Email;
                userExists.FirstName = user.FirstName;
                userExists.LastName = user.LastName;
                userExists.Gender = user.Gender;
                userExists.AccountType = user.AccountType;

                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            
            return NoContent();
        }



        // Delete Operation
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new {message = $"User with id {id} does not exist"});
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Private method to check if user exist
        private bool UserExists(int id)
        {
            var result = _context.Users.Any(x => x.Id == id);
            return result;
        }
    }
}
