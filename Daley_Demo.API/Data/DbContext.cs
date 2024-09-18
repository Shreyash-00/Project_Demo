using System.Data.Entity;
using Daley_Demo.API.Models;

namespace Daley_Demo.API.Data
{
    public class ECommerceDbContext : DbContext
    {
       
        public DbSet<User> Users { get; set; }
    
        public ECommerceDbContext() : base("name=ECommerceDbContext") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<User>().ToTable("Users");
           
        }
    }
}