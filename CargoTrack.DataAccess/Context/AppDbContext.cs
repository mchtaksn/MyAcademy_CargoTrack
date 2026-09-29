using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cargo - Sender / Receiver
            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.Sender)
                .WithMany(c => c.SentCargos)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.Receiver)
                .WithMany(c => c.ReceivedCargos)
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cargo - Origin / Destination Branch
            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.OriginBranch)
                .WithMany(c => c.OriginCargos)
                .HasForeignKey(c => c.OriginBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.DestinationBranch)
                .WithMany(c => c.DestinationCargos)
                .HasForeignKey(c => c.DestinationBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee - AppUser / Branch
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.AppUser)
                .WithMany()
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // CargoMovement - Cargo / Branch / TransferCenter / Employee
            modelBuilder.Entity<CargoMovement>()
                .HasOne(cm => cm.Cargo)
                .WithMany()
                .HasForeignKey(cm => cm.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(cm => cm.Branch)
                .WithMany(b => b.CargoMovements)
                .HasForeignKey(cm => cm.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(cm => cm.TransferCenter)
                .WithMany(tc => tc.CargoMovements)
                .HasForeignKey(cm => cm.TransferCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(cm => cm.Employee)
                .WithMany(e => e.CargoMovements)
                .HasForeignKey(cm => cm.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CargoPrice - Cargo (1:1)
            modelBuilder.Entity<CargoPrice>()
                .HasOne(cp => cp.Cargo)
                .WithOne()
                .HasForeignKey<CargoPrice>(cp => cp.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Delivery - Cargo (1:1) / Employee
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Cargo)
                .WithOne()
                .HasForeignKey<Delivery>(d => d.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // DeliveryException - Cargo / Employee
            modelBuilder.Entity<DeliveryException>()
                .HasOne(de => de.Cargo)
                .WithMany()
                .HasForeignKey(de => de.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeliveryException>()
                .HasOne(de => de.Employee)
                .WithMany()
                .HasForeignKey(de => de.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditLog - AppUser
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.AppUser)
                .WithMany()
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                    var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CargoMovement> CargoMovements { get; set; }
        public DbSet<TransferCenter> TransferCenters { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CargoPrice> CargoPrices { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryException> DeliveryExceptions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PricingRule> PricingRules { get; set; }
    }
}