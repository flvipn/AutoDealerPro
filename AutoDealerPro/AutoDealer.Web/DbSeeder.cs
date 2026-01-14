using AutoDealer.Data;
using AutoDealer.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoDealer.Web
{
    public static class DbSeeder
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();


                context.Database.EnsureCreated();

                if (context.Cars.Any())
                {
                    // Daca exista deja masini, nu mai facem nimic.
                    return;
                }


                string filePath = @"C:\HDD\Facultate\An 1\Semestru 1\Covaci\Laboratoare\Project\train.csv";

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[EROARE CRITICA] Nu gasesc CSV-ul la: {filePath}");
                    return;
                }

                var brands = new Dictionary<string, Brand>();
                var models = new Dictionary<string, Model>();
                var fuels = new Dictionary<string, Fuel>();
                var transmissions = new Dictionary<string, Transmission>();
                var carsToAdd = new List<Car>();

                Console.WriteLine("--> [INFO] Incepem citirea fisierului CSV complet...");

                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine();
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = ParseCsvLine(line);
                        if (values.Count < 13) continue;

                        string brandName = values[1].Trim();
                        string modelName = values[2].Trim();
                        string fuelName = string.IsNullOrEmpty(values[5].Trim()) ? "Unknown" : values[5].Trim();
                        string transName = string.IsNullOrEmpty(values[7].Trim()) ? "Unknown" : values[7].Trim();

                        // --- Brand ---
                        if (!brands.TryGetValue(brandName, out var brandEntity))
                        {
                            brandEntity = new Brand { Name = brandName };
                            brands[brandName] = brandEntity;
                            context.Brands.Add(brandEntity);
                        }

                        // --- Model ---
                        string modelKey = $"{brandName}_{modelName}";
                        if (!models.TryGetValue(modelKey, out var modelEntity))
                        {
                            modelEntity = new Model { Name = modelName, Brand = brandEntity };
                            models[modelKey] = modelEntity;
                            context.Models.Add(modelEntity);
                        }

                        // --- Fuel ---
                        if (!fuels.TryGetValue(fuelName, out var fuelEntity))
                        {
                            fuelEntity = new Fuel { Name = fuelName };
                            fuels[fuelName] = fuelEntity;
                            context.Fuels.Add(fuelEntity);
                        }

                        // --- Transmission ---
                        if (!transmissions.TryGetValue(transName, out var transEntity))
                        {
                            transEntity = new Transmission { Name = transName };
                            transmissions[transName] = transEntity;
                            context.Transmissions.Add(transEntity);
                        }

                        // Parsare Valori Numerice
                        int hp = ParseHorsepower(values[6]);
                        double volume = ParseEngineVolume(values[6]);
                        int.TryParse(values[3], out int year);
                        int.TryParse(values[4], out int mileage);
                        double.TryParse(values[12], out double price);

                        var car = new Car
                        {
                            Model = modelEntity,
                            Fuel = fuelEntity,
                            Transmission = transEntity,
                            Year = year,
                            Mileage = mileage,
                            Price = price,
                            Horsepower = hp,
                            EngineVolume = volume,
                            ExteriorColor = values[8],
                            InteriorColor = values[9],
                            HasAccident = values[10] != "None reported"
                        };
                        carsToAdd.Add(car);
                    }
                }


                context.SaveChanges();

                Console.WriteLine($"--> [INFO] Salvam {carsToAdd.Count} masini in baza de date (Batching)...");

                // Salvam in transe de cate 5000 ca sa nu se blocheze RAM-ul
                int batchSize = 5000;
                for (int i = 0; i < carsToAdd.Count; i += batchSize)
                {
                    var batch = carsToAdd.Skip(i).Take(batchSize).ToList();
                    context.Cars.AddRange(batch);
                    context.SaveChanges();
                    Console.WriteLine($"--> Salvat batch: {i} / {carsToAdd.Count}");
                }

                Console.WriteLine("--> [SUCCES] Import complet finalizat!");
            }
        }

        // --- Helpers ---
        private static List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string currentValue = "";
            foreach (char c in line)
            {
                if (c == '"') inQuotes = !inQuotes;
                else if (c == ',' && !inQuotes) { result.Add(currentValue); currentValue = ""; }
                else currentValue += c;
            }
            result.Add(currentValue);
            return result;
        }

        private static int ParseHorsepower(string text)
        {
            var match = Regex.Match(text, @"(\d+\.?\d*)HP");
            return match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) ? (int)v : 0;
        }

        private static double ParseEngineVolume(string text)
        {
            var match = Regex.Match(text, @"(\d+\.?\d*)L");
            return match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) ? v : 0.0;
        }
    }
}