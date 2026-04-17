using Microsoft.EntityFrameworkCore;

namespace UtulkyApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<Dog> Dogs { get; set; }
    }
}