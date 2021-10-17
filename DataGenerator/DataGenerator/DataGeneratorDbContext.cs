using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataGenerator
{
    public class DataGeneratorDbContext : DbContext
    {
        public DbSet<Table_Employee> Table_Employee { get; set; }

        public DbSet<Table_Department> Table_Department { get; set; }
        public DataGeneratorDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=test_data;Trusted_Connection=True;");
        }
    }
}
