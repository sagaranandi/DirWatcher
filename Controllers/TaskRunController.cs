using DirWatcher.Data;
using DirWatcher.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TaskRunController> _logger;

        /// <summary>
        /// Constructor for TaskRunController.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public TaskRunController(DirWatcherDbContext dbContext, ILogger<TaskRunController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogInformation("Retrieving TaskRun with ID: {ID}", id);

                var taskRun = await _dbContext.TaskRun.FindAsync(id);

                if (taskRun == null)
                {
                    _logger.LogWarning("TaskRun with ID {ID} not found", id);
                    return NotFound();
                }

                return taskRun;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving TaskRun with ID: {ID}", id);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
