using AutoDealer_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static AutoDealer_API.CarPriceModel;

namespace AutoDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {

        [HttpPost]
        public ActionResult<float> Predict([FromBody] CarPriceModel.ModelInput input)
        {
            try
            {
                CarPriceModel.ModelOutput result = CarPriceModel.Predict(input);
                return Ok(result.Score);
            }
            catch (Exception ex)
            {
                // Daca crapa ceva, returnam eroare 400
                return BadRequest($"Eroare la predictie: {ex.Message}");
            }
        }
    }
}