using Microsoft.AspNetCore.Mvc;
using UsedAndReliableCars.Models;

namespace UsedAndReliableCars.Controllers
{
    public class HomeController : Controller
    {
        public List<UsedCar> usedCars = new List<UsedCar>
        {
            new UsedCar
            {
                CarId = "Mazda6",
                Type = "Sedan",
                PriceCategory = 10000,
                Year = 2016,
                Make = "Mazda",
                Model = "Mazda6"
            },
            new UsedCar
            {
                CarId = "ToyotaCorolla",
                Type = "Sedan",
                PriceCategory = 15000,
                Year = 2019,
                Make = "Toyota",
                Model = "Corolla"
            },
            new UsedCar
            {
                CarId = "ChevroletEquinox",
                Type = "SUV",
                PriceCategory = 15000,
                Year = 2019,
                Make = "Chevrolet",
                Model = "Equinox"
            },
            new UsedCar
            {
                CarId = "ToyotaCorollaHybrid",
                Type = "Hybrid Sedan",
                PriceCategory = 20000,
                Year = 2021,
                Make = "Toyota",
                Model = "Corolla Hybrid"
            },
            new UsedCar
            {
                CarId = "SubaruCrosstrek",
                Type = "SUV",
                PriceCategory = 20000,
                Year = 2020,
                Make = "Subaru",
                Model = "Crosstrek"
            },
            new UsedCar
            {
                CarId = "ToyotaRAV4Hybrid",
                Type = "SUV",
                PriceCategory = 20000,
                Year = 2018,
                Make = "Toyota",
                Model = "RAV4 Hybrid"
            },
            new UsedCar
            {
                CarId = "ToyotaHighlander",
                Type = "SUV",
                PriceCategory = 20000,
                Year = 2018,
                Make = "Toyota",
                Model = "Highlander"
            },
            new UsedCar
            {
                CarId = "LexusNX",
                Type = "SUV",
                PriceCategory = 20000,
                Year = 2018,
                Make = "Lexus",
                Model = "NX"
            },
            new UsedCar
            {
                CarId = "MazdaMX5Miata",
                Type = "Sports Car",
                PriceCategory = 20000,
                Year = 2021,
                Make = "Mazda",
                Model = "MX-5 Miata"
            },
            new UsedCar
            {
                CarId = "HondaRidgeline",
                Type = "Truck",
                PriceCategory = 25000,
                Year = 2020,
                Make = "Honda",
                Model = "Ridgeline"
            }
        };
        public IActionResult Index()
        {
            var model = new UsedCar
            {
                UsedCars = usedCars // your list
            };

            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
