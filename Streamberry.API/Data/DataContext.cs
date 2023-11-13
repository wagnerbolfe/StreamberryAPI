using Microsoft.EntityFrameworkCore;
using Streamberry.API.Entities;

namespace Streamberry.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Streaming> Streamings { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasIndex(u => u.Title).IsUnique();
        }
        

    }
}
