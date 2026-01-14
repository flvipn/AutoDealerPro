using System.ComponentModel.DataAnnotations;

namespace AutoDealer.Data.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // un Brand are multe modele 1-m
        public ICollection<Model> Models { get; set; }
    }
}