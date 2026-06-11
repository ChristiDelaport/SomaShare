using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    public class Genre
    {
        [Key] 
        public int Genre_Id { get; set; }

        [Required] 
        public string Genre_Name { get; set; }


        // Navigation 
        public ICollection<Textbook> Textbooks { get; set; } = new List<Textbook>();
    }
}