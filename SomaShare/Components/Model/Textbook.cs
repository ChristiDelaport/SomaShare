using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomaShare.Components.Model
{
    public class Textbook
    {
        [Key]
        public int Textbook_ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Do not exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Do not exceed 100 characters")]
        public string Author { get; set; }

        [StringLength(20, ErrorMessage = "Do not exceed 20 characters")]
        public string? ISBN { get; set; }

        [StringLength(50, ErrorMessage = "Do not exceed 50 characters")]
        public string? Version { get; set; }

        [StringLength(500, ErrorMessage = "Do not exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public int Genre_ID { get; set; }
        public Genre Genre { get; set; }

        // Navigation properties
        public ICollection<ListingAd> ListingAds { get; set; } = new List<ListingAd>();
        public ICollection<WantedAd> WantedAds { get; set; } = new List<WantedAd>();
    }
}
