using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;

        public ProjectController(TaskManagementDbContext context)
        {
            _context = context;
        }

        // Create Operation
        [HttpPost]
        public async Task<ActionResult> CreateProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // Fetch By Id Operation
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            // Check if user is exist in the db
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }



        // Fetch All Operation
        [HttpGet]
        public async Task<List<Project>> GetAllProjects()
        {
            return await _context.Projects
                .Include(c => c.CreatedBy)
                //.Include(t => t.TodoTask)
                .ToListAsync();
        }



        // Update Operation




        // Delete Operation
    }
}
