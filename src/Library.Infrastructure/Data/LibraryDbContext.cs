using Microsoft.EntityFrameworkCore;
using Library.Core.Models;

namespace Library.Infrastructure.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.IsEnabled).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Author).HasMaxLength(100);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Genre).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.AvailableQuantity).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();

                entity.HasIndex(e => e.ISBN);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Genre);
                entity.HasIndex(e => e.IsDeleted);

                // Foreign key relationships
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.CreatedByUserId).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
