using System;
using Dashboard.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dashboard
{
    public class DashboardContext : DbContext
    {
        public DbSet<Home> Homes { get; set; }

        public DbSet<Work> Works { get; set; }

        public DbSet<Human> Humans { get; set; }

        public DbQuery<CountingResult> CountingResults { get; set; }

        public DashboardContext() {}

        public DashboardContext(DbContextOptions<DashboardContext> options)
            : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-BO0QG98\SQLEXPRESS;Database=Dashboard;Integrated Security=SSPI");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Home>().HasData(
                new Home { Id = 1 },
                new Home { Id = 2 },
                new Home { Id = 3 }
            );

            modelBuilder.Entity<Work>().HasData(
                new Work { Id = 1, Salary = 1000, InvitationDate = new DateTime() },
                new Work { Id = 2, Salary = null, InvitationDate = new DateTime() },
                new Work { Id = 3, Salary = 1000, InvitationDate = null }
            );

            modelBuilder.Entity<Human>().HasData(
                new Human { Id = 1, Name = "Test", Email = "test@mail.ru" },
                new Human { Id = 2, Name = "", Email = "test@mail.ru" },
                new Human { Id = 3, Name = null, Email = "test@mail.ru" }
            );
        }
    }
}
