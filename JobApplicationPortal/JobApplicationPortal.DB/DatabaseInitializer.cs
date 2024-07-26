using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pinewood.Customers.Db
{
    public class DatabaseInitializer
    {
        private readonly JobApplicationPortalDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(JobApplicationPortalDbContext context, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Apply any pending migrations automatically when the application starts.
        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                    _logger.LogInformation("Pending migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying migrations.");
                throw;
            }
        }
    }
}
