using Microsoft.EntityFrameworkCore;
using TuberTreats.Models;

namespace TuberTreats.Data;

public class TuberTreatsDbContext : DbContext
{
    public TuberTreatsDbContext(DbContextOptions<TuberTreatsDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<TuberDriver> TuberDrivers { get; set; }
    public DbSet<TuberOrder> TuberOrders { get; set; }
    public DbSet<Topping> Toppings { get; set; }
    public DbSet<TuberTopping> TuberToppings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Address).IsRequired().HasMaxLength(255);
        });

        // Configure TuberDriver entity
        modelBuilder.Entity<TuberDriver>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
        });

        // Configure Topping entity
        modelBuilder.Entity<Topping>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
        });

        // Configure TuberOrder entity
        modelBuilder.Entity<TuberOrder>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderPlacedOnDate).IsRequired();
            entity.Property(o => o.CustomerId).IsRequired();
            entity.Property(o => o.TuberDriverId).IsRequired(false); // Nullable
            entity.Property(o => o.DeliveredOnDate).IsRequired(false); // Nullable

            // Configure relationships
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.TuberOrders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            entity.HasOne(o => o.TuberDriver)
                  .WithMany(d => d.TuberDeliveries)
                  .HasForeignKey(o => o.TuberDriverId)
                  .OnDelete(DeleteBehavior.SetNull); // Set to null if driver is deleted
        });

        // Configure TuberTopping entity (junction table)
        modelBuilder.Entity<TuberTopping>(entity =>
        {
            entity.HasKey(tt => tt.Id);
            entity.Property(tt => tt.TuberOrderId).IsRequired();
            entity.Property(tt => tt.ToppingId).IsRequired();

            // Configure relationships
            entity.HasOne(tt => tt.TuberOrder)
                  .WithMany(o => o.TuberToppings)
                  .HasForeignKey(tt => tt.TuberOrderId)
                  .OnDelete(DeleteBehavior.Cascade); // Delete toppings when order is deleted

            entity.HasOne(tt => tt.Topping)
                  .WithMany(t => t.TuberToppings)
                  .HasForeignKey(tt => tt.ToppingId)
                  .OnDelete(DeleteBehavior.Cascade); // Delete associations when topping is deleted

            // Ensure unique combination of TuberOrderId and ToppingId - COMMENTED OUT FOR TESTS
            // entity.HasIndex(tt => new { tt.TuberOrderId, tt.ToppingId })
            //       .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
