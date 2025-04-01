using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace EMPLOYEE_MANAGER.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Empleado> Empleados { get; set; }

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
        ) : base(options)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var preference = _httpContextAccessor.HttpContext?.Request.Cookies["ConnectionPreference"];
        var connectionString = preference == "Remote"
            ? _configuration.GetConnectionString("RemoteConnection")
            : _configuration.GetConnectionString("LocalConnection");

        optionsBuilder.UseSqlServer(connectionString);
    }
}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
