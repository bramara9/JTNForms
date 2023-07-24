using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.DataModels;

public partial class DapperPocDbContext : DbContext
{
    public DapperPocDbContext()
    {
    }

    public DapperPocDbContext(DbContextOptions<DapperPocDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Window> Windows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(40)
                .IsFixedLength();
            entity.Property(e => e.CustomerStatus)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(40)
                .IsFixedLength();
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.State)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.BlindType)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.FabricName)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.RoomName)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<Window>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Window");

            entity.Property(e => e.ControlType)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Notes).HasMaxLength(50);
            entity.Property(e => e.Option)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.WindowName)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
