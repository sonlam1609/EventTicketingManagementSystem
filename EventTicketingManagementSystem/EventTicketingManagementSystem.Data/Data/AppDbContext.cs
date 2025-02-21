﻿using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Models.BaseModels;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Seat> Seats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithOne(s => s.Ticket)
                .HasForeignKey<Ticket>(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for User model
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // Indexes for Booking model
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.ExpiryDate)
                .HasDatabaseName("IX_Bookings_ExpiryDate");

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.UserId)
                .HasDatabaseName("IX_Bookings_UserId");

            // Indexes for Event model
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Name)
                .HasDatabaseName("IX_Events_Name");

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Category)
                .HasDatabaseName("IX_Events_Category");

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Status)
                .HasDatabaseName("IX_Events_Status");

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.StartDate)
                .HasDatabaseName("IX_Events_StartDate");

            // Indexes for Payment model
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.Status)
                .HasDatabaseName("IX_Payments_Status");

            // Indexes for Seat model
            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.Status)
                .HasDatabaseName("IX_Seats_Status");

            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.Row)
                .HasDatabaseName("IX_Seats_Row");

            // Indexes for Ticket model
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique()
                .HasDatabaseName("IX_Tickets_TicketNumber");

            // Indexes for UserRole model
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => ur.UserId)
                .HasDatabaseName("IX_UserRoles_UserId");
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EntityAuditBase<int> &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (EntityAuditBase<int>)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EntityAuditBase<int> &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (EntityAuditBase<int>)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
