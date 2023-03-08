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
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasData
                (
                   new Company
                   {
                       Id = new Guid("f8102623-58ee-419b-a0ec-6a8994e2c32d"),
                       Name = "IT_Soluctions Ltd",
                       Address = "583 Wall Dr. Gwyn Oak, MD 21207",
                       Country = "USA"
                   },
                   new Company
                   {
                       Id = new Guid("41ca47d3-0c31-47c4-bb61-7aad1cca61e5"),
                       Name = "Admin_Solutions Ltd",
                       Address = "312 Forest Avenue, BF 923",
                       Country = "USA"
                   }
                );
        }
    }
}
