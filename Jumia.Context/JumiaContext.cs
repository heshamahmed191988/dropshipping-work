using Jumia.Model;
using Jumia.Dtos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jumia.model;

namespace Jumia.Context
{
    public class JumiaContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> products { get; set; }
        public DbSet<Address> addresses { get; set; }
        public DbSet<OrderProduct> orderProducts { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<OrderAddress> orderAddresses { get; set; }


        public JumiaContext() : base()
        {

        }
        public JumiaContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableSensitiveDataLogging(false);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<OrderProduct>()
          .HasOne(c => c.Product) // Assuming CartItem has a navigation property called Product
          .WithMany(p => p.Orders) // Assuming Product has a collection property called CartItems
          .HasForeignKey(c => c.ProductId) // Assuming CartItem has a foreign key called ProductId
          .OnDelete(DeleteBehavior.Restrict);

            //     modelBuilder.Entity<Order>()
            //.HasOne(o => o.Address) 
            //.WithMany(a => a.Orders) 
            //.HasForeignKey(o => o.AddressId) 
            //.OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Order>()
       .HasOne(o => o.Address)
       .WithMany(a => a.Orders)
       .HasForeignKey(o => o.AddressId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<OrderAddress>()
                .HasOne(oa => oa.Order)
                .WithMany(o => o.OrderAddresses)
                .HasForeignKey(oa => oa.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete



            modelBuilder
                .Entity<Item>().Property(s=>s.Id).UseIdentityColumn();

            modelBuilder.Entity<Item>().HasOne(a=>a.Product)
                .WithMany(a=>a.items)
                .HasForeignKey(a=>a.ProductID);


        }


    }
}
