using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomaShare.Components.Model
{
    public class Offer
    {
        [Key]
        public int Offer_ID { get; set; }

        [Required(ErrorMessage = "Listing is required")]
        public int ListingAd_ID { get; set; }
        public ListingAd ListingAd { get; set; }

        [Required(ErrorMessage = "Buyer is required")]
        public string Buyer_ID { get; set; }
        public User Buyer { get; set; }

        [Required(ErrorMessage = "Seller is required")]
        public string Seller_ID { get; set; }
        public User Seller { get; set; }

        [Required(ErrorMessage = "Offer price is required")][Range(0.01, 10000, ErrorMessage = "Offer price must be between 0.01 and 10000")]
        public decimal OfferedPrice { get; set; }

        [StringLength(500, ErrorMessage = "Do not exceed 500 characters")]
        public string? Message { get; set; }

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateResponded { get; set; }

        [Required] [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected, Expired

        public int? Transaction_ID { get; set; }
        public Transaction? Transaction { get; set; }

    }
}
