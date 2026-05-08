using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    public class User : IdentityUser
    {
        [Required]
        public int User_Id { get; set; }
        [Required]
        public string First_Name { get; set; }

        [Required]
        public string Last_Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public int Role_ID { get; set; }


        [Required]
        public DateTime EnrollmentDate { get; set; }

        // Navigation
        public ICollection<ListingAd> ListingAds { get; set; } = new List<ListingAd>();
        public ICollection<WantedAd> WantedAds { get; set; } = new List<WantedAd>();
    }
}