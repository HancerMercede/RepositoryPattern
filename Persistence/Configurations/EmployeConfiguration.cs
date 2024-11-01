using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    public class EmployeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasData
                (
                    new Employee
                    { 
                       Id= new Guid("1ec472a4-cb64-439e-968e-8b1d4f7aa2ea"),
                       Name = "Sam Raiden",
                       Age = 26,
                       Position ="software developer",
                       CompanyId = new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d")
                
                    },

                    new Employee 
                    { 
                      Id= new Guid("022c78e3-7699-4af9-91c3-85dcf66d1d3a"),
                      Name = "Jana Mcleaf",
                      Age = 30,
                      Position ="software developer",
                      CompanyId = new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d")
                    },

                    new Employee
                    { 
                      Id= new Guid("270fa6f5-bb6b-420f-a025-2b8d13007f40"),
                      Name="Kane Miller",
                      Age = 35,
                      Position ="Administrator",
                      CompanyId = new Guid("41ca47d3-0c31-47c4-bb61-7aad1cca61e5")
                    }
                );
        }
    }
}
