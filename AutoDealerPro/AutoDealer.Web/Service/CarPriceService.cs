using AutoDealer.Web.DTOs;
using System.Text.Json;
using System.Text;

namespace AutoDealer.Web.Services
{
    public class CarPriceService
    {
        private readonly HttpClient _httpClient;

        public CarPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7032");
        }

        public async Task<double> GetPricePredictionAsync(PredictionInput input)
        {
            try
            {
                // Serializam datele in JSON
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(input),
                    Encoding.UTF8,
                    "application/json");

                // Trimitem cererea POST catre API
                var response = await _httpClient.PostAsync("api/Prediction", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // (Pretul)
                    var resultString = await response.Content.ReadAsStringAsync();

                    // Convertim punctul in virgula daca e cazul
                    if (double.TryParse(resultString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double price))
                    {
                        return price;
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}