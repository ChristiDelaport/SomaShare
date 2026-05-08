using SomaShare.Components.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class WantedAd
{
    [Key]
    public int WantedAd_ID { get; set; }

    [Required(ErrorMessage = "User is required")]
    public string User_ID { get; set; }
    public User User { get; set; }

    [Required(ErrorMessage = "Textbook is required")]
    public int Textbook_ID { get; set; }
    public Textbook Textbook { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    public int? Genre_ID { get; set; }
    public Genre Genre { get; set; }

    [Column(TypeName = "decimal(10,2)")] [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal? MaxPrice { get; set; }

    [StringLength(500, ErrorMessage = "Do not exceed 500 characters")]
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date_Posted { get; set; }

    [Column(TypeName = "date")]
    public DateTime? Date_Updated { get; set; }

    public bool IsActive { get; set; } = true;
}
