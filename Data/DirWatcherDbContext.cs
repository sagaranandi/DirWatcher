using DirWatcher.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace DirWatcher.Data
{
    /// <summary>
    /// Represents the database context for DirWatcher application.
    /// </summary>
    public class DirWatcherDbContext : DbContext
    {
        private readonly ILogger<DirWatcherDbContext> _logger;

        /// <summary>
        /// Constructor for DirWatcherDbContext.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        /// <param name="logger">Logger instance.</param>
        public DirWatcherDbContext(DbContextOptions<DirWatcherDbContext> options, ILogger<DirWatcherDbContext> logger) : base(options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the DbSet of TaskRun entities.
        /// </summary>
        public DbSet<TaskRun> TaskRun { get; set; }

        /// <summary>
        /// Called when the context is being configured.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                _logger.LogDebug("Configuring DirWatcherDbContext");
                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during DbContext configuration");
                throw; // rethrow the exception after logging
            }
        }

        /// <summary>
        /// Called when the model for a derived context has been initialized, but before the model has been locked down and used to initialize the context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                _logger.LogDebug("Initializing model for DirWatcherDbContext");
                base.OnModelCreating(modelBuilder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during model creation");
                throw; // rethrow the exception after logging
            }
        }
    }
}
