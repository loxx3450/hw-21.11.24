using hw_21._11._24.Models;
using Microsoft.EntityFrameworkCore;

namespace hw_21._11._24.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { 
            Database.EnsureCreated();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
