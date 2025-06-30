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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Many-to-many: ItemPackage  
            modelBuilder.Entity<ItemPackage>()
                .HasKey(ip => new { ip.ItemId, ip.PackageId });

            modelBuilder.Entity<ItemPackage>()
                .HasOne(ip => ip.Item)
                .WithMany(i => i.ItemPackages)
                .HasForeignKey(ip => ip.ItemId);

            modelBuilder.Entity<ItemPackage>()
                .HasOne(ip => ip.Package)
                .WithMany(p => p.ItemPackages)
                .HasForeignKey(ip => ip.PackageId);

            // ✅ User → Cart (One-to-Many) with Restrict to avoid cascade path issues  
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Package → Cart (One-to-One) with Restrict  
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Package)
                .WithOne(p => p.Cart)
                .HasForeignKey<Cart>(c => c.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ User → Package (One-to-Many) with Restrict to avoid multiple cascade paths  
            modelBuilder.Entity<Package>()
                .HasOne(p => p.User)
                .WithMany(u => u.Packages)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // (Optional) Package → Cake and Package → 3DModel can stay as Cascade  
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
        }
    }
}
