using SomaShare.Components.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    [Key]
    public int Transaction_Id { get; set; }

    [Required(ErrorMessage = "Offer is required")]
    public int Offer_Id { get; set; }
    public Offer Offer { get; set; }

    [Required(ErrorMessage = "Buyer is required")]
    public string Buyer_Id { get; set; }
    public User Buyer { get; set; }

    [Required(ErrorMessage = "Seller is required")]
    public string Seller_Id { get; set; }
    public User Seller { get; set; }

    [Required(ErrorMessage = "Transaction amount is required")] [Range(0.01, 10000, ErrorMessage = "Must be between 0.01 and 10000")]
    public decimal Transaction_Amount { get; set; }

    [Column(TypeName = "date")] //ref column thingie
    public DateTime Date_Created { get; set; } = DateTime.Now;

    [Column(TypeName = "date")]
    public DateTime? Date_Completed { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

    [Required]
    [StringLength(50)]
    public string Payment_Method { get; set; }

    // Navigation 
    public ICollection<Review>? Reviews { get; set; } = new List<Review>();
}
