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

            modelBuilder.UseSerialColumns();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // if (!optionsBuilder.IsConfigured)
            // {
            //     var configuration = new ConfigurationBuilder()
            //         .SetBasePath(Directory.GetCurrentDirectory())
            //         .AddJsonFile("appsettings.json")
            //         .Build();
            //
            //     var connectionString = configuration.GetConnectionString("DefaultConnection");
            //     optionsBuilder.UseNpgsql(connectionString);
            // }
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