using DirWatcher;
using Microsoft.EntityFrameworkCore;

public class DbConnectionChecker
{
    private readonly DirWatcherDbContext _dbContext;

    public DbConnectionChecker(DirWatcherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsDbConnectionSuccessful()
    {
        try
        {
            // Try querying the database to check the connection
            _dbContext.TaskRun.FirstOrDefault();
            return true; // If no exception occurs, the connection is successful
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Database connection failed: {ex.Message}");
            return false;
        }
    }
}
