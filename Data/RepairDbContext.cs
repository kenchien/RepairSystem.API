using Microsoft.EntityFrameworkCore;
using RepairSystem.API.Models;

namespace RepairSystem.API.Data
{
    public class RepairDbContext : DbContext
    {
        public RepairDbContext(DbContextOptions<RepairDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RepairTicket> RepairTickets { get; set; } = null!;
        public DbSet<Equipment> Equipment { get; set; } = null!;
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;
        public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = null!;
        public DbSet<AttachmentFile> AttachmentFiles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RepairTicket>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RepairTicket>()
                .HasOne(r => r.Handler)
                .WithMany()
                .HasForeignKey(r => r.HandledBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 