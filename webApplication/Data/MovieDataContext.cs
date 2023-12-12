using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using webApplication.Models;

namespace webApplication.Data
{
    /// <summary>
    /// Represents the database context for the Movie web application, providing
    /// access to the movie, user, and related data
    /// </summary>
    public class MovieDataContext : DbContext
    {
        /// <summary>Gets or sets the database set for movies.</summary>
        public DbSet<Movie> Movies { get; set; }
        
        /// <summary>database set for stars</summary>
        public DbSet<Star> Stars { get; set; }
        
        /// <summary>database set for directors</summary>
        public DbSet<Director> Directors { get; set; }
        
        /// <summary>database set for people</summary>
        public DbSet<Person> People { get; set; }
        
        /// <summary>database set for users</summary>
        
        public DbSet<User> Users { get; set; }
        
        /// <summary>database set for user's favorite movies</summary>
        public DbSet<UserMovieList> FavoriteMovieList { get; set; }
        
        /// <summary>database set for comments</summary>
        public DbSet<Comment> Comments { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieDataContext"/> class using the specified options
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/></param>
#pragma warning disable 8618
        public MovieDataContext(DbContextOptions<MovieDataContext> options) : base(options)
#pragma warning restore 8618
        {
            
        }

        /// <summary>
        /// Configures the schema needed for the context before the model is locked down and used to,
        /// initialize the context
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
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

        /// <summary>
        /// Configures the database to be used for this context
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context</param>
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