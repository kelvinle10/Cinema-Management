using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace CinemaManagementSystem.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A684DF9465");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4602FC81F").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D82FB6C63B");

            entity.ToTable("Customer");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movie__4BD2941AE823426F");

            entity.ToTable("Movie");

            entity.Property(e => e.AgeRating).HasMaxLength(10);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__32863939A1E87245");

            entity.ToTable("Room");

            entity.HasIndex(e => e.RoomName, "UQ__Room__6B500B55A46D3882").IsUnique();

            entity.Property(e => e.RoomName).HasMaxLength(50);
            entity.Property(e => e.RoomType).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713F36F8A1547");

            entity.ToTable("Seat");

            entity.HasIndex(e => new { e.RoomId, e.SeatNumber }, "UQ__Seat__2C64E5887331A4BC").IsUnique();

            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.SeatType).HasMaxLength(20);

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__RoomId__6754599E");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__Showtime__32D31F2006CB1A3A");

            entity.ToTable("Showtime");

            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtime__MovieI__6A30C649");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Showtime__RoomId__6B24EA82");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__712CC607618B0EBA");

            entity.ToTable("Ticket");

            entity.HasIndex(e => new { e.ShowtimeId, e.SeatId }, "UQ__Ticket__11C26E1E1CB541C3").IsUnique();

            entity.Property(e => e.BookingTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__Customer__71D1E811");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__SeatId__70DDC3D8");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__Showtime__6FE99F9F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
