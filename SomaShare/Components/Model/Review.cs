using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomaShare.Components.Model
{
    public class Review
    {
        [Key]
        public int Review_ID { get; set; }

        [Required(ErrorMessage = "Transaction is required")]
        public int Transaction_ID { get; set; }
        public Transaction Transaction { get; set; }

        [Required(ErrorMessage = "Reviewer is required")]
        public string Reviewer_ID { get; set; }
        public User Reviewer { get; set; }

        [Required(ErrorMessage = "Reviewee is required")]
        public string Reviewee_ID { get; set; }
        public User Reviewee { get; set; }

        [Required(ErrorMessage = "Rating is required")] [Range(1, 5, ErrorMessage = "Must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Do not exceed 500 characters")]
        public string? Comment { get; set; }

        public DateTime Date_Created { get; set; } = DateTime.Now;

    }
}
