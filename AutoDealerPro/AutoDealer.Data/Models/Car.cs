using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoDealer.Data.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        // FK Model (care ne da si Brand-ul implicit)
        public int ModelId { get; set; }
        [ForeignKey("ModelId")]
        public Model Model { get; set; }

        // FK Fuel
        public int FuelId { get; set; }
        [ForeignKey("FuelId")]
        public Fuel Fuel { get; set; }

        // FK Transmission
        public int TransmissionId { get; set; }
        [ForeignKey("TransmissionId")]
        public Transmission Transmission { get; set; }

        // Date numerice (Importante pt ML)
        public int Year { get; set; }
        public int Mileage { get; set; }

        public double Price { get; set; }

        // Extrase din coloana 'engine'
        public int Horsepower { get; set; }
        public double EngineVolume { get; set; } 

        // Estetice
        [MaxLength(50)]
        public string? ExteriorColor { get; set; }
        [MaxLength(50)]
        public string? InteriorColor { get; set; }

        public bool HasAccident { get; set; }
    }
}