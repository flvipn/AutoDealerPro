using System.ComponentModel.DataAnnotations;

namespace AutoDealer.Data.Models
{
    public class Fuel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}