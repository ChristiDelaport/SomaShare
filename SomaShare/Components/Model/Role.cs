using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    public class Role
    {
        [Key]
        public int Role_ID { get; set; }
        public string Role_name { get; set; }

    }
}
