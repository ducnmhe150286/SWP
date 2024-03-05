using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWP.Models
{
    public partial class SWPContext : DbContext
    {
        public SWPContext()
        {
        }

        public SWPContext(DbContextOptions<SWPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<CartItem> CartItems { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<FeedBack> FeedBacks { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Orderdetail> Orderdetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductDetail> ProductDetails { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;database=SWP;Integrated security=true;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("Blog");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Blog_User");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.BrandName).HasMaxLength(255);

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateBy).HasMaxLength(255);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.DetailId, e.CustomerId });

                entity.ToTable("CartItem");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartItem_User");

                entity.HasOne(d => d.Detail)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.DetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartItem_ProductDetail");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryName).HasMaxLength(255);

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateBy).HasMaxLength(255);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("Color");

                entity.Property(e => e.ColorName).HasMaxLength(255);

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.ToTable("FeedBack");

                entity.Property(e => e.FeedBackId).HasColumnName("FeedBackID");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedBacks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_FeedBack_User");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.CustomerName).HasMaxLength(255);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.ShipAddress).HasMaxLength(20);

                entity.Property(e => e.ShipDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Orderdetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.DetailId });

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdateBy).HasMaxLength(255);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Orderdetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orderdetails_Order");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.UpdateBy).HasMaxLength(255);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Category");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__ProductD__135C316D80D9C867");

                entity.ToTable("ProductDetail");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateBy).HasMaxLength(255);

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_ProductDetail_Color");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductDetail_Products");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK_ProductDetail_Size");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImage");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdateBy).HasMaxLength(255);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductImage_Products");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RoleName).HasMaxLength(255);

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("Size");

                entity.Property(e => e.SizeId).HasColumnName("SizeID");

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SizeName).HasMaxLength(255);

                entity.Property(e => e.UpdateBy).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.CreatedBy).HasMaxLength(255);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Otp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("otp");

                entity.Property(e => e.OtpExpired)
                    .HasColumnType("datetime")
                    .HasColumnName("otp_expired");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.UpdatedBy).HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
