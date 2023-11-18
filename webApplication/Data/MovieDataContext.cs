using Microsoft.EntityFrameworkCore;
using webApplication.Models;

namespace webApplication.Data
{
    public class MovieDataContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Person> People { get; set; }
        
        public MovieDataContext(DbContextOptions<MovieDataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
        
        
    }
}