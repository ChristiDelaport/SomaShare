using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;

namespace SomaShare.Components.Model
{

    /// Implements EF Core's IDbContextFactory pattern for dependency injection.
  
    public class DesignTimeDbContextFactory : IDbContextFactory<SomaContext>
    {
        private readonly DbContextOptions<SomaContext> _options;

        public DesignTimeDbContextFactory(DbContextOptions<SomaContext> options)
        {
            _options = options;
        }

        public SomaContext CreateDbContext()
        {
            return new SomaContext(_options);
        }
    }
}
