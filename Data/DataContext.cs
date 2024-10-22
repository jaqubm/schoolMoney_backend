using Microsoft.EntityFrameworkCore;

namespace schoolMoney_backend.Data;

public class DataContext(IConfiguration config) : DbContext
{
    //public virtual DbSet<User> User { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Returning if connection is already set up
        if (optionsBuilder.IsConfigured) return;
        
        // Setting up connection with DB
        optionsBuilder.UseSqlServer(config.GetConnectionString("AzureSQL"),
            o =>
            {
                o.EnableRetryOnFailure();
                o.CommandTimeout(180000);
            });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("calorieCounter");
        
        // Connecting Entities with DB
        // modelBuilder.Entity<User>()
            // .HasKey(u => u.Email);
    }
}