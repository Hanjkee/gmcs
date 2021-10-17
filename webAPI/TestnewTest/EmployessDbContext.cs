using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestnewTest
{
    public class EmployeeDbContext : DbContext
    {
        public DbSet<Table_Employee> Table_Employee { get; set; }

        public DbSet<Table_Department> Table_Department { get; set; }
        public EmployeeDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=test_data;Trusted_Connection=True;");
        }
    }
}
