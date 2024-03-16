using DirWatcher.DataModel;
using Microsoft.EntityFrameworkCore;

namespace DirWatcher.Data
{
    /// <summary>
    /// Represents the database context for DirWatcher application.
    /// </summary>
    public class DirWatcherDbContext : DbContext
    {
        /// <summary>
        /// Constructor for DirWatcherDbContext.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public DirWatcherDbContext(DbContextOptions<DirWatcherDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet of TaskRun entities.
        /// </summary>
        public DbSet<TaskRun> TaskRun { get; set; }
    }
}
