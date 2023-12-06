using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using webApplication.Models;

namespace webApplication.Data
{
    public class MovieDataContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Star> Stars { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Person> People { get; set; }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserMovieList> FavoriteMovieList { get; set; }
        
        public DbSet<Comment> Comments { get; set; }
        public MovieDataContext(DbContextOptions<MovieDataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for Director
            modelBuilder.Entity<Director>()
                .HasKey(d => new {d.MovieId, d.PersonId});

            // Composite key for Star
            modelBuilder.Entity<Star>()
                .HasKey(s => new {s.MovieId, s.PersonId});

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
            });
            
            // Composite key for UserMovieList
            modelBuilder.Entity<UserMovieList>()
                .HasKey(uml => new { uml.UserId, uml.MovieId });

            modelBuilder.Entity<UserMovieList>()
                .HasOne(uml => uml.User)
                .WithMany(u => u.UserFavoriteMovieList)
                .HasForeignKey(uml => uml.UserId);
            
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId);
            
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);
            
            modelBuilder.UseSerialColumns();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var conn = configuration.GetConnectionString("DefaultConnection");
            if (conn == null)
            {
                throw new Exception("ConnectionString environment variable not found");
            }

            optionsBuilder.UseNpgsql(conn);
        }
    }
}