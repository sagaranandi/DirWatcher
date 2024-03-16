using DirWatcher.Data;
using DirWatcher.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
        private readonly ILogger<ConfigurationController> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for ConfigurationController.
        /// </summary>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="backgroundServiceWorker">Background service worker instance.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="serviceProvider">Service provider instance.</param>
        public ConfigurationController(IConfiguration configuration, BackgroundServiceWorker backgroundServiceWorker, ILogger<ConfigurationController> logger, IServiceProvider serviceProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _backgroundServiceWorker = backgroundServiceWorker ?? throw new ArgumentNullException(nameof(backgroundServiceWorker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider;
        }

        private BackgroundServiceWorker GetBackgroundServiceWorker()
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
                _logger.LogInformation("Setting directory configuration to {Directory}", directory);

                _configuration["DirWatcher:Directory"] = directory;
                ((IConfigurationRoot)_configuration).Reload();

                var backgroundServiceWorker = GetBackgroundServiceWorker();
                if (backgroundServiceWorker != null)
                {
                    backgroundServiceWorker.UpdateDirectory(directory);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting directory configuration");
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
                _logger.LogInformation("Setting magic string configuration to {MagicString}", magicString);

                _configuration["DirWatcher:MagicString"] = magicString;
                ((IConfigurationRoot)_configuration).Reload();

                var backgroundServiceWorker = GetBackgroundServiceWorker();
                if (backgroundServiceWorker != null)
                {
                    backgroundServiceWorker.UpdateMagicString(magicString);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting magic string configuration");
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
                _logger.LogInformation("Starting directory monitoring");

                _backgroundServiceWorker.StartMonitoring();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting directory monitoring");
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
                _logger.LogInformation("Stopping directory monitoring");

                _backgroundServiceWorker.StopMonitoring();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while stopping directory monitoring");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
