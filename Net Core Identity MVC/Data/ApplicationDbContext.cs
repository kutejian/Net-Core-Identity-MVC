using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Net_Core_Identity_MVC.Data.Configuration;
using Net_Core_Identity_MVC.Entitys;

namespace Net_Core_Identity_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Employee>  Employees { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new EmployeeConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
