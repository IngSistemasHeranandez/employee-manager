using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EMPLOYEE_MANAGER.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }

        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

}
