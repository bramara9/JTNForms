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

        modelBuilder.Entity<LookUp>(entity =>
        {
            entity.Property(e => e.Type).IsFixedLength();
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
