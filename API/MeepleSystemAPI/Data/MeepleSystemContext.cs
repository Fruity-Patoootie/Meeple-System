using System;
using System.Collections.Generic;
using MeepleSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MeepleSystemAPI.Data;

public partial class MeepleSystemContext : DbContext
{
    private readonly IConfiguration _config;
    public MeepleSystemContext(IConfiguration config)
    {
        _config = config;
    }

    public MeepleSystemContext(DbContextOptions<MeepleSystemContext> options, IConfiguration config)
        : base(options)
    {
        _config = config;
    }


    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameCategory> GameCategories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Style> Styles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TimePlayed> TimesPlayed { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = _config["MeepleSystem:DatabaseConnectionString"];
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__category__D54EE9B4D0C03FE2");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__game__FFE11FCFBE83497F");

            entity.ToTable("game");

            entity.HasIndex(e => e.Barcode, "IDX_game_game_barcode");

            entity.HasIndex(e => e.Title, "IDX_game_game_title");

            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("barcode");
            entity.Property(e => e.BestPlayers)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("best_players");
            entity.Property(e => e.BggGameId).HasColumnName("BGG_game_id");
            entity.Property(e => e.Cost)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("cost");
            entity.Property(e => e.DateAcquired).HasColumnName("date_acquired");
            entity.Property(e => e.Expansion).HasColumnName("expansion");
            entity.Property(e => e.ImageLocation)
                .HasColumnType("text")
                .HasColumnName("image_location");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.MaxDuration).HasColumnName("max_duration");
            entity.Property(e => e.MaxPlayers).HasColumnName("max_players");
            entity.Property(e => e.MinDuration).HasColumnName("min_duration");
            entity.Property(e => e.MinPlayers).HasColumnName("min_players");
            entity.Property(e => e.NeedsAddedToBgg).HasColumnName("needs_added_to_BGG");
            entity.Property(e => e.RecommendedPlayers)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("recommended_players");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.StyleId).HasColumnName("style_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Location).WithMany(p => p.Games)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("fk_game_location_id_ref_location_location_id");

            entity.HasOne(d => d.Seller).WithMany(p => p.Games)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("fk_seller_id_ref_seller_seller_id");

            entity.HasOne(d => d.Style).WithMany(p => p.Games)
                .HasForeignKey(d => d.StyleId)
                .HasConstraintName("fk_game_table_style_id_ref_style_style_id");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Games)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_supplier_id_ref_supplier_supplier_id");
        });

        modelBuilder.Entity<GameCategory>(entity =>
        {
            entity.HasKey(e => e.GameCategoryId).HasName("PK__game_cat__E67B462CEE60CEED");

            entity.ToTable("game_category");

            entity.Property(e => e.GameCategoryId).HasColumnName("game_category_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");

            entity.HasOne(d => d.Category).WithMany(p => p.GameCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("game_category_fk_category_id_ref_category_category_id");

            entity.HasOne(d => d.Game).WithMany(p => p.GameCategories)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("game_category_fk_game_id_ref_game_game_id");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__location__771831EAFD0A3B3A");

            entity.ToTable("location");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.LocationName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("location_name");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__seller__780A0A970922BD1F");

            entity.ToTable("seller");

            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.SellerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("seller_name");
        });

        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.StyleId).HasName("PK__style__D333B397E1EE222F");

            entity.ToTable("style");

            entity.Property(e => e.StyleId).HasColumnName("style_id");
            entity.Property(e => e.StyleName)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("style_name");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E8CBA4B486");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<TimePlayed>(entity =>
        {
            entity.HasKey(e => e.TimePlayedId).HasName("PK__time_pla__8AF61CAE916F4910");

            entity.ToTable("time_played");

            entity.HasIndex(e => e.GameId, "IDK_time_played_game_id");

            entity.Property(e => e.TimePlayedId).HasColumnName("time_played_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.Game).WithMany(p => p.TimesPlayed)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("time_played_fk_game_id_ref_game_game_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user__B9BE370F6AB935FC");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
