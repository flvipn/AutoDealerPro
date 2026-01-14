using Grpc.Core;

namespace AutoDealer.GRPC.Services
{

    public class CurrencyService : CurrencyConverter.CurrencyConverterBase
    {

        private const double UsdToEurRate = 0.92;
        private const double UsdToRonRate = 4.60;
        public override Task<CurrencyResponse> GetConvertedPrices(CurrencyRequest request, ServerCallContext context)
        {
            // Facem calculele
            var response = new CurrencyResponse
            {
                PriceInEur = request.PriceInUsd * UsdToEurRate,
                PriceInRon = request.PriceInUsd * UsdToRonRate
            };

            return Task.FromResult(response);
        }
    }
}