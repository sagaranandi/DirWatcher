using Microsoft.EntityFrameworkCore;

namespace DirWatcher
{
    public class DirWatcherDbContext : DbContext
    {
        public DirWatcherDbContext(DbContextOptions<DirWatcherDbContext> options) : base(options)
        {
        }

        public DbSet<TaskRun> TaskRun { get; set; }
    }
}
