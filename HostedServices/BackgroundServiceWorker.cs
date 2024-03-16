using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DirWatcher.Data;
using DirWatcher.DataModel;

namespace DirWatcher.Services
{
    /// <summary>
    /// Background service for watching a directory and processing its files.
    /// </summary>
    public class BackgroundServiceWorker : BackgroundService
    {
        private readonly ILogger<BackgroundServiceWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private string _directory;
        private string _magicString;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isMonitoring;

        /// <summary>
        /// Constructor for BackgroundServiceWorker.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="configuration">Configuration instance.</param>
        /// <param name="serviceScopeFactory">Service scope factory instance.</param>
        public BackgroundServiceWorker(ILogger<BackgroundServiceWorker> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _directory = _configuration["DirWatcher:Directory"] ?? throw new InvalidOperationException("Directory path not found in configuration.");
            _magicString = _configuration["DirWatcher:MagicString"] ?? throw new InvalidOperationException("Magic string not found in configuration.");
            _cancellationTokenSource = new CancellationTokenSource();
            _isMonitoring = true;
        }

        /// <summary>
        /// Updates the magic string used for monitoring.
        /// </summary>
        /// <param name="magicString">The new magic string to use for monitoring.</param>
        public void UpdateMagicString(string magicString)
        {
            _magicString = magicString;
        }

        /// <summary>
        /// Updates the directory being monitored.
        /// </summary>
        /// <param name="directory">The new directory path to monitor.</param>
        public void UpdateDirectory(string directory)
        {
            _directory = directory;
        }

        /// <summary>
        /// Starts or resumes monitoring the directory.
        /// </summary>
        public void StartMonitoring()
        {
            _isMonitoring = true;
        }

        /// <summary>
        /// Stops monitoring the directory.
        /// </summary>
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Monitoring directory: {Directory}", _directory);

                    var previousRun = await GetPreviousRunAsync();
                    var files = Directory.GetFiles(_directory);

                    if (IsChangesDetected(previousRun, files))
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<DirWatcherDbContext>();
                            await ProcessDirectoryAsync(dbContext, previousRun, files);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing directory.");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

        private async Task<TaskRun> GetPreviousRunAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DirWatcherDbContext>();
                return await dbContext.TaskRun.OrderByDescending(tr => tr.EndTime).FirstOrDefaultAsync();
            }
        }

        private async Task ProcessDirectoryAsync(DirWatcherDbContext dbContext, TaskRun previousRun, string[] files)
        {
            var currentRun = new TaskRun
            {
                StartTime = DateTime.Now,
                FilesAdded = new List<string>(),
                FilesDeleted = new List<string>(),
                FilesModified = new List<string>(),
                MagicStringCount = 0,
                Status = "InProgress"
            };

            if (previousRun != null)
            {
                var filesBefore = previousRun.FilesAdded != null ? previousRun.FilesAdded : new List<string>();

                var filesAdded = files.Except(filesBefore).ToList();
                var filesDeleted = filesBefore.Except(files).ToList();

                currentRun.FilesAdded.AddRange(filesAdded);
                currentRun.FilesDeleted.AddRange(filesDeleted);
            }

            var modifiedFiles = files.Where(file => IsFileModified(file)).Select(Path.GetFileName).ToList();
            currentRun.FilesModified.AddRange(modifiedFiles);

            foreach (var file in files)
            {
                try
                {
                    var content = File.ReadAllText(file);
                    currentRun.MagicStringCount += CountOccurrences(content, _magicString);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing file: {file}");
                }
            }

            currentRun.EndTime = DateTime.Now;
            currentRun.Status = "Success";

            dbContext.TaskRun.Add(currentRun);
            await dbContext.SaveChangesAsync();
        }

        private int CountOccurrences(string content, string searchString)
        {
            return content.Split(new[] { searchString }, StringSplitOptions.None).Length - 1;
        }

        private bool IsFileModified(string filePath)
        {
            var lastModifiedBefore = File.GetLastWriteTimeUtc(filePath);
            var lastModifiedAfter = File.GetLastWriteTimeUtc(filePath);

            return lastModifiedBefore != lastModifiedAfter;
        }

        private bool IsChangesDetected(TaskRun previousRun, string[] files)
        {
            if (previousRun == null)
                return true;

            var filesBefore = previousRun.FilesAdded;

            return !files.SequenceEqual(filesBefore);
        }
    }
}
