using DirWatcher.Data;
using DirWatcher.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DirWatcher.Controllers
{
    

    /// <summary>
    /// Controller for configuring directory and magic string settings and managing directory monitoring.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BackgroundServiceWorker _backgroundServiceWorker;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for ConfigurationController.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="backgroundServiceWorker">Background service worker instance.</param>
        public ConfigurationController(IConfiguration configuration, BackgroundServiceWorker backgroundServiceWorker, IServiceProvider serviceProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _backgroundServiceWorker = backgroundServiceWorker ?? throw new ArgumentNullException(nameof(backgroundServiceWorker));
            _serviceProvider = serviceProvider;
        }
        private BackgroundServiceWorker GetPeriodicTaskService()
        {
            return _serviceProvider.GetServices<IHostedService>()
                .OfType<BackgroundServiceWorker>()
                .FirstOrDefault();
        }
        /// <summary>
        /// Sets the directory configuration.
        /// </summary>
        /// <param name="directory">The directory path to set.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("ConfigureDirectory")]
        public IActionResult SetDirectory([FromBody] string directory)
        {
            try
            {
                // Set the directory configuration
                _configuration["DirWatcher:Directory"] = directory;
                // Refresh the configuration to pick up changes
                ((IConfigurationRoot)_configuration).Reload();
                // Update directory in BackgroundServiceWorker if it's running
                var backgroundServiceWorker = _serviceProvider.GetService<BackgroundServiceWorker>();
                if (backgroundServiceWorker != null)
                {
                    backgroundServiceWorker.UpdateDirectory(directory);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

     


        /// <summary>
        /// Sets the magic string configuration.
        /// </summary>
        /// <param name="magicString">The magic string to set.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("ConfigureMagicString")]
        public IActionResult SetMagicString([FromBody] string magicString)
        {
            try
            {
                // Set the magic string configuration
                _configuration["DirWatcher:MagicString"] = magicString;
                // Refresh the configuration to pick up changes
                ((IConfigurationRoot)_configuration).Reload();
                // Update magic string in BackgroundServiceWorker if it's running
                var backgroundServiceWorker = _serviceProvider.GetService<BackgroundServiceWorker>();
                if (backgroundServiceWorker != null)
                {
                    backgroundServiceWorker.UpdateMagicString(magicString);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts monitoring the directory.
        /// </summary>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("StartMonitoring")]
        public IActionResult StartMonitoring()
        {
            try
            {
                _backgroundServiceWorker.StartMonitoring();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops monitoring the directory.
        /// </summary>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("StopMonitoring")]
        public IActionResult StopMonitoring()
        {
            try
            {
                _backgroundServiceWorker.StopMonitoring();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
