using SomaShare.Components.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ListingAd
{
    [Key] 
    public int ListingAd_ID { get; set; }

    [Required(ErrorMessage = "User is required")] 
    public string User_ID { get; set; }
    public User User { get; set; }

    [Required(ErrorMessage = "Textbook is required")] 
    public int Textbook_ID { get; set; }
    public Textbook Textbook { get; set; }

    [Required(ErrorMessage = "Genre is required")] 
    public int? Genre_ID { get; set; }
    public Genre Genre { get; set; }

    [Required(ErrorMessage = "Price is required")][Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")] 
    public decimal Price { get; set; } 
    
    [Required(ErrorMessage = "Condition is required")][StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
    public string Condition { get; set; } 

    [StringLength(100, ErrorMessage = "Campus location cannot exceed 100 characters")]
    public string? CampusLocation { get; set; }

    [StringLength(500, ErrorMessage = "Do not exceed 500 characters")]
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date_Posted { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Date_Updated { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation 
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
