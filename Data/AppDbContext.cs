using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MVCProject.Models;

namespace MVCProject.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
       public DbSet<User> Users { get; set; }
       public DbSet<Product>Products { get; set; }
       public DbSet<Cart> Carts { get; set; }
         public DbSet<Order> Orders { get; set; }
    }
}
