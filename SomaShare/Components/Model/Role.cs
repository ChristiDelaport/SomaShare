using System.ComponentModel.DataAnnotations;

namespace SomaShare.Components.Model
{
    public class Role
    {
        [Key]
        public int Role_Id { get; set; }
        public string Role_name { get; set; }

    }
}
