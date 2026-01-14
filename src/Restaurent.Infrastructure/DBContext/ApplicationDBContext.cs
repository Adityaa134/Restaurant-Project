using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;

namespace Restaurent.Infrastructure.DBContext
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Address { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Dish>().ToTable("Dishes");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Carts>().ToTable("Carts");
            modelBuilder.Entity<Order>() 
                .ToTable("Orders")
                .HasOne(o=>o.User)
                .WithMany(o=>o.Orders)
                .HasForeignKey(o=>o.UserId)
                .OnDelete(DeleteBehavior.Restrict); //if a user is deleted then his orders should still exist   

            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<Address>().ToTable("Address");
        }
    }
}
