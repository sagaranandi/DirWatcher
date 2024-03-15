using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DirWatcher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _directory;
        private readonly string _magicString;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _directory = _configuration["DirWatcher:Directory"]; // Read directory path from configuration
            _magicString = _configuration["DirWatcher:MagicString"]; // Read magic string from configuration
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Retrieve previous run
                    var previousRun = await GetPreviousRunAsync();

                    // Retrieve list of files in the directory
                    var files = Directory.GetFiles(_directory);

                    // Process directory
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DirWatcherDbContext>();
                        await ProcessDirectoryAsync(dbContext, previousRun, files);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing directory.");
                }

                // Delay for a specific interval before executing the next iteration
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
                MagicStringCount = 0,
                Status = "InProgress"
            };

            // Detect changes in files
            if (previousRun != null)
            {
                var filesBefore = previousRun.FilesAdded;
                var filesAfter = files.Select(Path.GetFileName);

                var filesAdded = filesAfter.Except(filesBefore).ToList();
                var filesDeleted = filesBefore.Except(filesAfter).ToList();

                currentRun.FilesAdded.AddRange(filesAdded);
                currentRun.FilesDeleted.AddRange(filesDeleted);

                // Count occurrences of magic string
                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    currentRun.MagicStringCount += CountOccurrences(content, _magicString);
                }
            }

            // Save task run details to database
            currentRun.EndTime = DateTime.Now;
            currentRun.Status = "Success";

            dbContext.TaskRun.Add(currentRun);
            await dbContext.SaveChangesAsync();
        }

        private int CountOccurrences(string content, string searchString)
        {
            // Logic to count occurrences of Magic String in content
            return content.Split(new[] { searchString }, StringSplitOptions.None).Length - 1;
        }
    }
}
