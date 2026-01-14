using System.Text.Json.Serialization;

namespace AutoDealer.Web.DTOs
{
    public class PredictionInput
    {
        // "Traducem" Year din C# in model_year pentru API
        [JsonPropertyName("model_year")]
        public float Year { get; set; }

        [JsonPropertyName("mileage")]
        public float Mileage { get; set; }

        // API-ul vrea "brand" cu litere mici
        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("fuel_type")]
        public string Fuel { get; set; }

        [JsonPropertyName("transmission")]
        public string Transmission { get; set; }

        [JsonPropertyName("engine")]
        public string Engine { get; set; }

        [JsonPropertyName("ext_col")]
        public string ExteriorColor { get; set; } = "Black";

        [JsonPropertyName("int_col")]
        public string InteriorColor { get; set; } = "Black";

        [JsonPropertyName("accident")]
        public string Accident { get; set; } = "No";

        [JsonPropertyName("clean_title")]
        public string CleanTitle { get; set; } = "Yes";
    }
}