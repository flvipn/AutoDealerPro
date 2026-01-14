using AutoDealer.Data;
using AutoDealer.Data.Models;
using AutoDealer.GRPC;
using AutoDealer.Web.DTOs;
using AutoDealer.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoDealer.Web.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CarPriceService _priceService;
        private readonly CurrencyConverter.CurrencyConverterClient _currencyClient;

        public CarsController(AppDbContext context, CarPriceService priceService, CurrencyConverter.CurrencyConverterClient currencyClient)
        {
            _context = context;
            _priceService = priceService;
            _currencyClient = currencyClient;
        }

        public async Task<IActionResult> Index(string searchString, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["PriceSortParm"] = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewData["YearSortParm"] = sortOrder == "Year" ? "year_desc" : "Year";

            var cars = _context.Cars
                               .Include(c => c.Model).ThenInclude(m => m.Brand)
                               .Include(c => c.Fuel)
                               .Include(c => c.Transmission)
                               .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c => c.Model.Brand.Name.Contains(searchString)
                                    || c.Model.Name.Contains(searchString)
                                    || (c.Model.Brand.Name + " " + c.Model.Name).Contains(searchString));
            }

            switch (sortOrder)
            {
                case "price_desc":
                    cars = cars.OrderByDescending(c => c.Price);
                    break;
                case "Year":
                    cars = cars.OrderBy(c => c.Year);
                    break;
                case "year_desc":
                    cars = cars.OrderByDescending(c => c.Year);
                    break;
                default:
                    cars = cars.OrderByDescending(c => c.Year);
                    break;
            }

            int pageSize = 20;
            return View(await PaginatedList<Car>.CreateAsync(cars, pageNumber ?? 1, pageSize));
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(c => c.Model).ThenInclude(m => m.Brand)
                .Include(c => c.Fuel)
                .Include(c => c.Transmission)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null) return NotFound();

            // 1.(Predictie Pret)
            try
            {
                var input = new PredictionInput
                {
                    Year = car.Year,
                    Mileage = car.Mileage,
                    Brand = car.Model.Brand.Name,
                    Model = car.Model.Name,
                    Fuel = car.Fuel.Name,
                    Transmission = car.Transmission.Name,
                    Engine = car.Horsepower + "HP",
                    ExteriorColor = car.ExteriorColor ?? "White",
                    InteriorColor = car.InteriorColor ?? "Black",
                    Accident = car.HasAccident ? "Yes" : "No",
                    CleanTitle = "Yes"
                };

                double predictedPrice = await _priceService.GetPricePredictionAsync(input);
                ViewBag.PredictedPrice = predictedPrice;
            }
            catch
            {
                ViewBag.PredictedPrice = 0d;
            }

            // 2. LOGICA gRPC
            try
            {
                // A. Convertim Pretul REAL (al vanzatorului)
                var realPriceRequest = new CurrencyRequest { PriceInUsd = (double)car.Price };
                var realPriceResponse = await _currencyClient.GetConvertedPricesAsync(realPriceRequest);

                ViewBag.PriceEur = realPriceResponse.PriceInEur;
                ViewBag.PriceRon = realPriceResponse.PriceInRon;

                // B. Convertim Pretul ESTIMAT
                if (ViewBag.PredictedPrice != null && (double)ViewBag.PredictedPrice > 0)
                {
                    var predRequest = new CurrencyRequest { PriceInUsd = (double)ViewBag.PredictedPrice };
                    var predResponse = await _currencyClient.GetConvertedPricesAsync(predRequest);

                    ViewBag.PredEur = predResponse.PriceInEur;
                    ViewBag.PredRon = predResponse.PriceInRon;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOG GRPC ERROR: " + ex.Message);

            }

            return View(car);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var car = await _context.Cars
                .Include(c => c.Model).ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            ViewData["ModelId"] = new SelectList(_context.Models.Include(m => m.Brand)
                                                .Select(m => new { Id = m.Id, FullName = m.Brand.Name + " " + m.Name })
                                                .OrderBy(m => m.FullName), "Id", "FullName");
            ViewData["FuelId"] = new SelectList(_context.Fuels, "Id", "Name");
            ViewData["TransmissionId"] = new SelectList(_context.Transmissions, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModelId,FuelId,TransmissionId,Year,Mileage,Price,Horsepower,EngineVolume,ExteriorColor,InteriorColor,HasAccident")] Car car)
        {
            ModelState.Remove("Model");
            ModelState.Remove("Fuel");
            ModelState.Remove("Transmission");
            ModelState.Remove("Brand");

            if (string.IsNullOrEmpty(car.ExteriorColor)) car.ExteriorColor = "Unknown";
            if (string.IsNullOrEmpty(car.InteriorColor)) car.InteriorColor = "Unknown";

            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ModelId"] = new SelectList(_context.Models.Include(m => m.Brand).Select(m => new { Id = m.Id, FullName = m.Brand.Name + " " + m.Name }), "Id", "FullName", car.ModelId);
            ViewData["FuelId"] = new SelectList(_context.Fuels, "Id", "Name", car.FuelId);
            ViewData["TransmissionId"] = new SelectList(_context.Transmissions, "Id", "Name", car.TransmissionId);
            return View(car);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            ViewData["ModelId"] = new SelectList(_context.Models.Include(m => m.Brand)
                                                .Select(m => new { Id = m.Id, FullName = m.Brand.Name + " " + m.Name })
                                                .OrderBy(m => m.FullName), "Id", "FullName", car.ModelId);
            ViewData["FuelId"] = new SelectList(_context.Fuels, "Id", "Name", car.FuelId);
            ViewData["TransmissionId"] = new SelectList(_context.Transmissions, "Id", "Name", car.TransmissionId);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModelId,FuelId,TransmissionId,Year,Mileage,Price,Horsepower,EngineVolume,ExteriorColor,InteriorColor,HasAccident")] Car car)
        {
            if (id != car.Id) return NotFound();
            ModelState.Remove("Model");
            ModelState.Remove("Fuel");
            ModelState.Remove("Transmission");
            ModelState.Remove("Brand");

            if (string.IsNullOrEmpty(car.ExteriorColor)) car.ExteriorColor = "Unknown";
            if (string.IsNullOrEmpty(car.InteriorColor)) car.InteriorColor = "Unknown";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Cars.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModelId"] = new SelectList(_context.Models.Include(m => m.Brand).Select(m => new { Id = m.Id, FullName = m.Brand.Name + " " + m.Name }), "Id", "FullName", car.ModelId);
            ViewData["FuelId"] = new SelectList(_context.Fuels, "Id", "Name", car.FuelId);
            ViewData["TransmissionId"] = new SelectList(_context.Transmissions, "Id", "Name", car.TransmissionId);
            return View(car);
        }
    }
}