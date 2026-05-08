using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    public class Genre
    {
        [Key] 
        public int Genre_ID { get; set; }

        [Required] 
        public string Genre_Name { get; set; }


        // Navigation 
        public ICollection<Textbook> Textbooks { get; set; } = new List<Textbook>();
    }
}