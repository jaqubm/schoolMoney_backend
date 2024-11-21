using Microsoft.EntityFrameworkCore;
using schoolMoney_backend.Models;

namespace schoolMoney_backend.Data;

public class DataContext(IConfiguration config) : DbContext
{
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Account> Account { get; set; }
    public virtual DbSet<Class> Class { get; set; }
    public virtual DbSet<Child> Child { get; set; }
    public virtual DbSet<Fundraise> Fundraise { get; set; }
    public virtual DbSet<Transaction> Transaction { get; set; }
    
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
        modelBuilder.HasDefaultSchema("schoolMoney");
        
        // User and Account (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Account)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AccountNumber)
                .OnDelete(DeleteBehavior.Restrict);

            // Fundraiser and Account (One-to-One)
            modelBuilder.Entity<Fundraise>()
                .HasOne(f => f.Account)
                .WithMany(a => a.Fundraises)
                .HasForeignKey(f => f.AccountNumber)
                .OnDelete(DeleteBehavior.Restrict);

            // Class and Treasurer (User) (One-to-Many)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Treasurer)
                .WithMany(u => u.ClassesAsTreasurer)
                .HasForeignKey(c => c.TreasurerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Child and Parent (User) (One-to-Many)
            modelBuilder.Entity<Child>()
                .HasOne(ch => ch.Parent)
                .WithMany(u => u.Children)
                .HasForeignKey(ch => ch.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Child and Class (One-to-Many)
            modelBuilder.Entity<Child>()
                .HasOne(ch => ch.Class)
                .WithMany(c => c.Children)
                .HasForeignKey(ch => ch.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fundraiser and Class (One-to-Many)
            modelBuilder.Entity<Fundraise>()
                .HasOne(f => f.Class)
                .WithMany(c => c.Fundraises)
                .HasForeignKey(f => f.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction and Fundraiser (One-to-Many)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Fundraise)
                .WithMany(f => f.Transactions)
                .HasForeignKey(t => t.FundraiseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction and User (One-to-Many)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}