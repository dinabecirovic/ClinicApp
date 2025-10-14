using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicApp.Data
{
    public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
    {
        public ClinicDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=ClinicApp;Trusted_Connection=True"
            );

            return new ClinicDbContext(optionsBuilder.Options);
        }
    }
}
