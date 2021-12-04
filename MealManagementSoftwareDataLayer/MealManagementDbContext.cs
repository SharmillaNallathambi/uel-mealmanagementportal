using System;
using MealManagementSoftwareDataLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MealManagementSoftwareDataLayer
{

    public partial class MealManagementDbContext : IdentityDbContext<ApplicationUser>
    {

        public MealManagementDbContext()
        {
        }

        public MealManagementDbContext(DbContextOptions<MealManagementDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Basket> Carts { get; set; }
        public virtual DbSet<Offer> Discounts { get; set; }
        public virtual DbSet<Dish> MealMenus { get; set; }
        public virtual DbSet<DishAvailability> MealMenuAvailabilities { get; set; }
        public virtual DbSet<DishType> MealTypes { get; set; }
        public virtual DbSet<DishOrder> Orders { get; set; }
        public virtual DbSet<DishOrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderPayment> Payments { get; set; }
        public virtual DbSet<CustomerProfile> UserProfiles { get; set; }
        public virtual DbSet<GardianLink> ParentMapping { get; set; }
        public virtual DbSet<PaymentCard> Cards { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("data source=localhost;initial catalog=canteensystemdb;MultipleActiveResultSets=True;App=EntityFramework;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Basket>(entity =>
            {
                entity.ToTable("Cart");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MealAvailableDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.MealMenu)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.MealMenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_MealMenus");
            });
             
            base.OnModelCreating(modelBuilder);
        }
    }
}
