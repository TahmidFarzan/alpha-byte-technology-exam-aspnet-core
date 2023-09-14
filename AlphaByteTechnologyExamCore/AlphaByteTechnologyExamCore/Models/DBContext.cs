using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaByteTechnologyExamCore.Models
{
    public class DBContext:DbContext
    {
        public DBContext() : base() { }

  

        public DbSet<Employee> tEmployee { get; set; }
        public DbSet<Department> tDepartment { get; set; }
        public DbSet<Division> tDivision { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=DESKTOP-6KIJ0HS\\SQLEXPRESS;Database=AlphaByteTechnologyExamCore;User Id=sa;Password=123;";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
