using Microsoft.EntityFrameworkCore;
using webApplication.Models;

namespace webApplication.Data;

public class UserDataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        modelBuilder.Entity<User>()
            .HasKey(d => new {d.UserId});
        
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