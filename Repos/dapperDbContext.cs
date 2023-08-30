﻿using System;
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

    public virtual DbSet<Fabric> Fabrics { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<LookUp> LookUps { get; set; }

    public virtual DbSet<ReorderWindow> ReorderWindows { get; set; }

    public virtual DbSet<Window> Windows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07D0A2DD76");

            entity.Property(e => e.CustomerStatus).IsFixedLength();
            entity.Property(e => e.PhoneNumber).IsFixedLength();
            entity.Property(e => e.State).IsFixedLength();
            entity.Property(e => e.Zip).IsFixedLength();
        });

        modelBuilder.Entity<Fabric>(entity =>
        {
            entity.Property(e => e.Image).IsFixedLength();
        });

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Resolution).IsFixedLength();

            entity.HasOne(d => d.Customer).WithMany(p => p.Issues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Issues__Customer__412EB0B6");

            entity.HasOne(d => d.Window).WithMany(p => p.Issues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Issues__WindowId__403A8C7D");
        });

        modelBuilder.Entity<LookUp>(entity =>
        {
            entity.Property(e => e.Type).IsFixedLength();
        });

        modelBuilder.Entity<ReorderWindow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReorderW__3214EC0749C6D731");

            entity.Property(e => e.BlindType).IsFixedLength();
            entity.Property(e => e.StackType).IsFixedLength();

            entity.HasOne(d => d.Window).WithMany(p => p.ReorderWindows)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReorderWi__Windo__4BAC3F29");
        });

        modelBuilder.Entity<Window>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Window__3214EC076711EE56");

            entity.Property(e => e.BlindType).IsFixedLength();
            entity.Property(e => e.StackType).IsFixedLength();
            entity.Property(e => e.WindowName).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
