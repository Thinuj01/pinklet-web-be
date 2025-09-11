using Microsoft.EntityFrameworkCore;
using pinklet.Models;

namespace pinklet.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<_3DCakeModel> Cakes3dModel { get; set; }
        public DbSet<CakeLayerModel> CakeLayers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Cake> Cakes { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<ItemPackage> ItemPackages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // ✅ Many-to-many: ItemPackage  
        //    modelBuilder.Entity<ItemPackage>()
        //        .HasKey(ip => new { ip.ItemId, ip.PackageId });

        //    modelBuilder.Entity<ItemPackage>()
        //        .HasOne(ip => ip.Item)
        //        .WithMany(i => i.ItemPackages)
        //        .HasForeignKey(ip => ip.ItemId);

        //    modelBuilder.Entity<ItemPackage>()
        //        .HasOne(ip => ip.Package)
        //        .WithMany(p => p.ItemPackages)
        //        .HasForeignKey(ip => ip.PackageId);

        //    // ✅ User → Cart (One-to-Many)
        //    modelBuilder.Entity<Cart>()
        //        .HasOne(c => c.User)
        //        .WithMany(u => u.Carts)
        //        .HasForeignKey(c => c.UserId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // ✅ Package → Cart (One-to-One)
        //    modelBuilder.Entity<Cart>()
        //        .HasOne(c => c.Package)
        //        .WithOne(p => p.Cart)
        //        .HasForeignKey<Cart>(c => c.PackageId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<Item>()
        //        .HasOne(i => i.Vendor)
        //        .WithMany(v => v.Items)  // requires you to add ICollection<Item> Items in Vendor.cs
        //        .HasForeignKey(i => i.VendorId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // ✅ User → Package (One-to-Many)
        //    modelBuilder.Entity<Package>()
        //        .HasOne(p => p.User)
        //        .WithMany(u => u.Packages)
        //        .HasForeignKey(p => p.UserId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // ✅ Package → Cake & 3DCake (Cascade)
        //    modelBuilder.Entity<Package>()
        //        .HasOne(p => p.Cake)
        //        .WithMany(c => c.Packages)
        //        .HasForeignKey(p => p.CakeId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<Package>()
        //        .HasOne(p => p.ThreeDCake)
        //        .WithMany()
        //        .HasForeignKey(p => p.ThreeDCakeId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // ✅ User → Vendor (One-to-One) Restrict (if 1 user can register as 1 vendor)
        //    modelBuilder.Entity<Vendor>()
        //        .HasOne(v => v.User)              
        //        .WithOne() // or .WithOne(u => u.Vendor) if you add navigation property in User
        //        .HasForeignKey<Vendor>(v => v.UserId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // ✅ User → Orders (One-to-Many)
        //    modelBuilder.Entity<Order>()
        //        .HasOne(o => o.User)
        //        .WithMany(u => u.Orders) // requires you to add ICollection<Order> Orders in User.cs
        //        .HasForeignKey(o => o.UserId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // ✅ Cart → Order (One-to-One)
        //    modelBuilder.Entity<Order>()
        //        .HasOne(o => o.Cart)
        //        .WithOne(c => c.Order) // requires you to add Order? Order in Cart.cs
        //        .HasForeignKey<Order>(o => o.CartId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Many-to-many: ItemPackage
            modelBuilder.Entity<ItemPackage>()
                .HasKey(ip => ip.Id);

            modelBuilder.Entity<ItemPackage>()
                .HasOne(ip => ip.Item)
                .WithMany(i => i.ItemPackages)
                .HasForeignKey(ip => ip.ItemId);

            modelBuilder.Entity<ItemPackage>()
                .HasOne(ip => ip.Package)
                .WithMany(p => p.ItemPackages)
                .HasForeignKey(ip => ip.PackageId);

            // ✅ User → Cart (One-to-Many)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Package → Cart (One-to-One)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Package)
                .WithOne(p => p.Cart)
                .HasForeignKey<Cart>(c => c.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Item → Vendor (Many-to-One)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Vendor)
                .WithMany(v => v.Items)
                .HasForeignKey(i => i.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ User → Package (One-to-Many)
            modelBuilder.Entity<Package>()
                .HasOne(p => p.User)
                .WithMany(u => u.Packages)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Package → Cake & 3DCake (Cascade)
            modelBuilder.Entity<Package>()
                .HasOne(p => p.Cake)
                .WithMany(c => c.Packages)
                .HasForeignKey(p => p.CakeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.ThreeDCake)
                .WithMany()
                .HasForeignKey(p => p.ThreeDCakeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ User → Vendor (One-to-One)
            modelBuilder.Entity<Vendor>()
                .HasOne(v => v.User)
                .WithOne()
                .HasForeignKey<Vendor>(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ User → Orders (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Cart → Order (One-to-One)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Cart)
                .WithOne(c => c.Order)
                .HasForeignKey<Order>(o => o.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ OrderItem → Order (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)   // you can also add ICollection<OrderItem> OrderItems in Order.cs
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ OrderItem → ItemPackage (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ItemPackage)
                .WithMany()   // or add ICollection<OrderItem> OrderItems in ItemPackage.cs
                .HasForeignKey(oi => oi.ItemPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ OrderItem → Vendor (Many-to-One)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Vendor)
                .WithMany()   // or add ICollection<OrderItem> OrderItems in Vendor.cs
                .HasForeignKey(oi => oi.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
