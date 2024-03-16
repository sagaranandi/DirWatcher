using DirWatcher.Data;
using DirWatcher.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DirWatcher.Controllers
{
    /// <summary>
    /// Controller for managing TaskRun entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TaskRunController : ControllerBase
    {
        private readonly DirWatcherDbContext _dbContext;

        /// <summary>
        /// Constructor for TaskRunController.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        public TaskRunController(DirWatcherDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Retrieves a TaskRun entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the TaskRun to retrieve.</param>
        /// <returns>An asynchronous action that returns an ActionResult containing the TaskRun if found, or NotFound if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskRun>> GetTaskRun(int id)
        {
            try
            {
                var taskRun = await _dbContext.TaskRun.FindAsync(id);

                if (taskRun == null)
                {
                    return NotFound();
                }

                return taskRun;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
