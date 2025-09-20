using System.ComponentModel.DataAnnotations;

namespace Library.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        
        [MaxLength(200)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int CreatedByUserId { get; set; }
        
        // Navigation property
        public User? CreatedByUser { get; set; }
    }

    public class Genre
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        
        [MaxLength(200)]
        public string? Description { get; set; }
        
        public MediaType? ApplicableToMediaType { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public int CreatedByUserId { get; set; }
        
        // Navigation property
        public User? CreatedByUser { get; set; }
    }
}
