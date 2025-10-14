using ClinicApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace ClinicApp
{
    public partial class App : Application
    {
        public static ClinicDbContext? DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ClinicApp;Trusted_Connection=True");

            DbContext = new ClinicDbContext(optionsBuilder.Options);
            DbContext.Database.EnsureCreated();
        }
    }
}