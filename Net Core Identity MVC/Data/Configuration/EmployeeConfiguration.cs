using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Net_Core_Identity_MVC.Entitys;

namespace Net_Core_Identity_MVC.Data.Configuration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
             builder.HasData(
                new Employee
                {
                    Id = 1,
                    Name = "John11",
                    Age = 16,
                    Position = "架构师"
                },
                new Employee
                {
                    Id = 2,
                    Name = "John22",
                    Age = 18,
                    Position = "普通人"
                }
            );
        }
    }
}
