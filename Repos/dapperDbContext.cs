using System;
using System.Collections.Generic;
using JTNForms.DataModels;
using Microsoft.EntityFrameworkCore;

namespace JTNForms.Repos;

public partial class dapperDbContext : DbContext
{
    public dapperDbContext()
    {
    }

    public dapperDbContext(DbContextOptions<dapperDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LookUp> LookUps { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Window> Windows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC078F611867");

            entity.Property(e => e.Address).IsFixedLength();
            entity.Property(e => e.Address2).IsFixedLength();
            entity.Property(e => e.Community).IsFixedLength();
            entity.Property(e => e.CustomerStatus).IsFixedLength();
            entity.Property(e => e.EmailAddress).IsFixedLength();
            entity.Property(e => e.FirstName).IsFixedLength();
            entity.Property(e => e.LastName).IsFixedLength();
            entity.Property(e => e.PhoneNumber).IsFixedLength();
            entity.Property(e => e.State).IsFixedLength();
            entity.Property(e => e.Zip).IsFixedLength();
        });

        modelBuilder.Entity<LookUp>(entity =>
        {
            entity.Property(e => e.Type).IsFixedLength();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Room__3214EC07D83A49CF");

            entity.Property(e => e.BlindType).IsFixedLength();
            entity.Property(e => e.FabricName).IsFixedLength();
            entity.Property(e => e.RoomName).IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.Rooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Room__CustomerId__6A30C649");
        });

        modelBuilder.Entity<Window>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Window__3214EC076B589E30");

            entity.Property(e => e.ControlType).IsFixedLength();
            entity.Property(e => e.Option).IsFixedLength();
            entity.Property(e => e.WindowName).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
