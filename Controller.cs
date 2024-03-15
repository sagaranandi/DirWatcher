using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DirWatcher.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("directory")]
        public IActionResult SetDirectory([FromBody] string directory)
        {
            // Set the directory configuration
            _configuration["DirWatcher:Directory"] = directory;
            return Ok();
        }

        [HttpPost("magicstring")]
        public IActionResult SetMagicString([FromBody] string magicString)
        {
            // Set the magic string configuration
            _configuration["DirWatcher:MagicString"] = magicString;
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TaskRunController : ControllerBase
    {
        private readonly DirWatcherDbContext _dbContext;

        public TaskRunController(DirWatcherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskRun>> GetTaskRun(int id)
        {
            var taskRun = await _dbContext.TaskRun.FindAsync(id);

            if (taskRun == null)
            {
                return NotFound();
            }

            return taskRun;
        }
    }
}
