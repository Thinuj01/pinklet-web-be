

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
    }

}