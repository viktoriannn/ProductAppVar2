using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductDemo.Models;

public partial class ProductShoesContext : DbContext
{
    public ProductShoesContext()
    {
    }

    public ProductShoesContext(DbContextOptions<ProductShoesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<ProductList> ProductLists { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Production> Productions { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Pvz> Pvzs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ProductShoes;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_100_CI_AI_KS_SC_UTF8");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory);

            entity.ToTable("Category");

            entity.Property(e => e.IdCategory)
                .ValueGeneratedNever()
                .HasColumnName("ID_Category");
            entity.Property(e => e.Category1)
                .HasMaxLength(255)
                .HasColumnName("Category");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.IdCity);

            entity.ToTable("City");

            entity.Property(e => e.IdCity)
                .ValueGeneratedNever()
                .HasColumnName("ID_City");
            entity.Property(e => e.City1)
                .HasMaxLength(255)
                .HasColumnName("City");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder);

            entity.ToTable("Order");

            entity.Property(e => e.IdOrder)
                .ValueGeneratedNever()
                .HasColumnName("ID_Order");
            entity.Property(e => e.DataDelivery).HasColumnName("Data_Delivery");
            entity.Property(e => e.DataOrder).HasColumnName("Data_Order");
            entity.Property(e => e.IdPvz).HasColumnName("ID_PVZ");
            entity.Property(e => e.IdUser).HasColumnName("ID_User");
            entity.Property(e => e.Status).HasMaxLength(255);

            entity.HasOne(d => d.IdPvzNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdPvz)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_PVZ");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem);

            entity.ToTable("OrderItem");

            entity.Property(e => e.IdOrderItem)
                .ValueGeneratedNever()
                .HasColumnName("ID_OrderItem");
            entity.Property(e => e.IdOrder).HasColumnName("ID_Order");
            entity.Property(e => e.IdProduct).HasColumnName("ID_Product");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_ProductList");
        });

        modelBuilder.Entity<ProductList>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("ProductList");

            entity.Property(e => e.IdProduct)
                .ValueGeneratedNever()
                .HasColumnName("ID_Product");
            entity.Property(e => e.Article).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Discount).HasColumnType("decimal(2, 0)");
            entity.Property(e => e.IdCategory).HasColumnName("ID_Category");
            entity.Property(e => e.IdProductType).HasColumnName("ID_Product_type");
            entity.Property(e => e.IdProduction).HasColumnName("ID_Production");
            entity.Property(e => e.IdProvider).HasColumnName("ID_Provider");
            entity.Property(e => e.IdUnit).HasColumnName("ID_Unit");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductList_Category");

            entity.HasOne(d => d.IdProductTypeNavigation).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.IdProductType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductList_Product_type");

            entity.HasOne(d => d.IdProductionNavigation).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.IdProduction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductList_Production");

            entity.HasOne(d => d.IdProviderNavigation).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.IdProvider)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductList_Provider");

            entity.HasOne(d => d.IdUnitNavigation).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.IdUnit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductList_Unit");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.IdProductType);

            entity.ToTable("Product_type");

            entity.Property(e => e.IdProductType)
                .ValueGeneratedNever()
                .HasColumnName("ID_Product_type");
            entity.Property(e => e.ProductType1)
                .HasMaxLength(255)
                .HasColumnName("Product_type");
        });

        modelBuilder.Entity<Production>(entity =>
        {
            entity.HasKey(e => e.IdProduction);

            entity.ToTable("Production");

            entity.Property(e => e.IdProduction)
                .ValueGeneratedNever()
                .HasColumnName("ID_Production");
            entity.Property(e => e.Production1)
                .HasMaxLength(255)
                .HasColumnName("Production");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.IdProvider);

            entity.ToTable("Provider");

            entity.Property(e => e.IdProvider)
                .ValueGeneratedNever()
                .HasColumnName("ID_Provider");
            entity.Property(e => e.Provider1)
                .HasMaxLength(255)
                .HasColumnName("Provider");
        });

        modelBuilder.Entity<Pvz>(entity =>
        {
            entity.HasKey(e => e.IdPvz);

            entity.ToTable("PVZ");

            entity.Property(e => e.IdPvz)
                .ValueGeneratedNever()
                .HasColumnName("ID_PVZ");
            entity.Property(e => e.HumberHouse)
                .HasMaxLength(255)
                .HasColumnName("Humber_house");
            entity.Property(e => e.IdCity).HasColumnName("ID_City");
            entity.Property(e => e.Street).HasMaxLength(255);

            entity.HasOne(d => d.IdCityNavigation).WithMany(p => p.Pvzs)
                .HasForeignKey(d => d.IdCity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PVZ_City");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.Property(e => e.IdRole)
                .ValueGeneratedNever()
                .HasColumnName("ID_Role");
            entity.Property(e => e.Role1)
                .HasMaxLength(255)
                .HasColumnName("Role");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.IdUnit);

            entity.ToTable("Unit");

            entity.Property(e => e.IdUnit)
                .ValueGeneratedNever()
                .HasColumnName("ID_Unit");
            entity.Property(e => e.Unit1)
                .HasMaxLength(255)
                .HasColumnName("Unit");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("ID_User");
            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.Login).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Patronymic).HasMaxLength(255);
            entity.Property(e => e.Surname).HasMaxLength(255);

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
