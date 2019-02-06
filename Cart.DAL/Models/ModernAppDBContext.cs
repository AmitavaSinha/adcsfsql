using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cart.DAL.Models
{
    public partial class ModernAppDBContext : DbContext
    {
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<OrderHeader> OrderHeader { get; set; }
        public virtual DbSet<Products> Products { get; set; }

        private readonly string _dbName;
        public ModernAppDBContext(string dbName)
        {
            _dbName = dbName;
        }

        public ModernAppDBContext(DbContextOptions<ModernAppDBContext> options)
            : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!string.IsNullOrEmpty(_dbName))
                optionsBuilder.UseSqlServer(_dbName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.OrderHeaderId).HasColumnName("OrderHeaderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.OrderHeader)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderHeaderId)
                    .HasConstraintName("FK_OrderDetails_OrderHeader");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<OrderHeader>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.OrderByUserId)
                    .HasColumnName("OrderByUserID")
                    .HasMaxLength(100);

                entity.Property(e => e.OrderByUserName).HasMaxLength(100);

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.ShippingAddress).HasMaxLength(200);

                entity.Property(e => e.Status).HasMaxLength(50);
            });
            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ProductCategory).HasMaxLength(200);

                entity.Property(e => e.ProductImage).HasMaxLength(200);
            });
        }
    }
}
