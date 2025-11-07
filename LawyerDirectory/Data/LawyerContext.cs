using Microsoft.EntityFrameworkCore;
using LawyerDirectory.Models;

namespace LawyerDirectory.Data
{
    public class LawyerContext : DbContext
    {
        public LawyerContext(DbContextOptions<LawyerContext> options)
            : base(options)
        {
        }

        public DbSet<Lawyer> Lawyers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create indexes for better search performance
            modelBuilder.Entity<Lawyer>()
                .HasIndex(l => l.State);

            modelBuilder.Entity<Lawyer>()
                .HasIndex(l => l.City);

            modelBuilder.Entity<Lawyer>()
                .HasIndex(l => l.YearsOfExperience);
        }
    }
}
