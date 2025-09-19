using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Core.Models
{
    public class MediaItem
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }
        
        [MaxLength(100)]
        public string? Author { get; set; }
        
        [MaxLength(20)]
        public string? ISBN { get; set; }
        
        public MediaType Type { get; set; }
        
        [MaxLength(50)]
        public string? Genre { get; set; }
        
        [MaxLength(50)]
        public string? Category { get; set; }
        
        public DateTime? PublicationDate { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Available quantity cannot be negative")]
        public int AvailableQuantity { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public int CreatedByUserId { get; set; }
        
        public int? UpdatedByUserId { get; set; }

        // Navigation property
        public User? CreatedByUser { get; set; }
        public User? UpdatedByUser { get; set; }

        // Business logic methods
        public bool IsAvailable => AvailableQuantity > 0 && !IsDeleted;
        
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Quantity cannot be negative");
                
            var difference = newQuantity - Quantity;
            Quantity = newQuantity;
            AvailableQuantity += difference;
            
            if (AvailableQuantity < 0)
                AvailableQuantity = 0;
        }
        
        public bool CanBorrow(int requestedQuantity = 1)
        {
            return IsAvailable && AvailableQuantity >= requestedQuantity;
        }
        
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        
        public void Restore()
        {
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum MediaType
    {
        Book = 1,
        Magazine = 2,
        DVD = 3,
        AudioBook = 4,
        EBook = 5,
        BluRay = 6,
        CD = 7,
        Journal = 8
    }

    public static class MediaTypeExtensions
    {
        public static string GetDisplayName(this MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.Book => "Book",
                MediaType.Magazine => "Magazine",
                MediaType.DVD => "DVD",
                MediaType.AudioBook => "Audio Book",
                MediaType.EBook => "E-Book",
                MediaType.BluRay => "Blu-ray",
                MediaType.CD => "CD",
                MediaType.Journal => "Journal",
                _ => mediaType.ToString()
            };
        }
        
        public static bool RequiresISBN(this MediaType mediaType)
        {
            return mediaType == MediaType.Book || mediaType == MediaType.EBook;
        }
        
        public static bool IsDigital(this MediaType mediaType)
        {
            return mediaType == MediaType.EBook || mediaType == MediaType.AudioBook;
        }
    }
}
