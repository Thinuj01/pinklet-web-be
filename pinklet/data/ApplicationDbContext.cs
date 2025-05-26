

using Microsoft.EntityFrameworkCore;
using pinklet.Models;

namespace pinklet.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } // Example table
    }

}