// ============================================================
// DATABASE CONTEXT - Entity Framework Core
// ============================================================
 
using Microsoft.EntityFrameworkCore;
using ShopWaveBlazor.Web.ECommerceAPI.Models;
 
namespace ShopWaveBlazor.Web.ECommerceAPI.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options) { }
 
        // ── DbSets ─────────────────────────────────────────
        public DbSet<User>         Users         { get; set; }
        public DbSet<Category>     Categories    { get; set; }
        public DbSet<Product>      Products      { get; set; }
        public DbSet<Cart>         Carts         { get; set; }
        public DbSet<Order>        Orders        { get; set; }
        public DbSet<OrderDetail>  OrderDetails  { get; set; }
        public DbSet<Payment>      Payments      { get; set; }
        public DbSet<Review>       Reviews       { get; set; }
        public DbSet<Notification> Notifications { get; set; }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
 
            // ── Unique constraints ─────────────────────────
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
 
            // Cart: one row per user-product pair
            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.UserID, c.ProductID }).IsUnique();
 
            // Review: one review per user-product pair
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserID, r.ProductID }).IsUnique();
 
            // ── Relationships ──────────────────────────────
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);
 
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(c => c.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);
 
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderID)
                .OnDelete(DeleteBehavior.Cascade);
 
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductID)
                .OnDelete(DeleteBehavior.Cascade);
 
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);
 
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);
 
 
            // ── Column precision ───────────────────────────
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(10,2)");
 
            modelBuilder.Entity<Product>()
                .Property(p => p.Rating)
                .HasColumnType("decimal(3,2)");
 
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(10,2)");
 
            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryCharges)
                .HasColumnType("decimal(10,2)");
 
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal(10,2)");
 
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");
        }
    }
}
