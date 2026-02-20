using Microsoft.AspNetCore.Mvc;
using UsedAndReliableCars.Models;
using System.Collections.Generic;

namespace UsedAndReliableCars.Controllers
{
    public class HomeController : Controller
    {
        private readonly List<UsedCar> usedCars = new()
        {
            new UsedCar { Type = "Sedan",        PriceCategory = 10000, Year = "2014-2021",    Make = "Mazda",      Model = "Mazda6" },
            new UsedCar { Type = "Sedan",        PriceCategory = 15000, Year = "2014-2019",    Make = "Toyota",     Model = "Corolla" },
            new UsedCar { Type = "SUV",          PriceCategory = 15000, Year = "2018-2024",    Make = "Chevrolet",  Model = "Equinox" },
            new UsedCar { Type = "Hybrid Sedan", PriceCategory = 20000, Year = "2020-present", Make = "Toyota",     Model = "Corolla Hybrid" },
            new UsedCar { Type = "SUV",          PriceCategory = 20000, Year = "2018-2023",    Make = "Subaru",     Model = "Crosstrek" },
            new UsedCar { Type = "SUV",          PriceCategory = 20000, Year = "2016-2018",    Make = "Toyota",     Model = "RAV4 Hybrid" },
            new UsedCar { Type = "SUV",          PriceCategory = 20000, Year = "2014-2019",    Make = "Toyota",     Model = "Highlander" },
            new UsedCar { Type = "SUV",          PriceCategory = 20000, Year = "2015-2021",    Make = "Lexus",      Model = "NX" },
            new UsedCar { Type = "Sports Car",   PriceCategory = 20000, Year = "2016-2024",    Make = "Mazda",      Model = "MX-5 Miata" },
            new UsedCar { Type = "Truck",        PriceCategory = 25000, Year = "2017-present", Make = "Honda",      Model = "Ridgeline" }
        };

        // GET: /
        public IActionResult Index()
        {
            var model = new UsedCar { UsedCars = usedCars };
            return View(model);
        }

        // GET: /Home/Car
        public IActionResult Car()
        {
            var model = new UsedCar { UsedCars = usedCars };
            return View(model);
        }

        // GET: /Home/CarModel?make=Toyota&model=Corolla
        public IActionResult CarModel(string make, string model)
        {
            var car = usedCars.FirstOrDefault(c =>
                c.Make.Equals(make, StringComparison.OrdinalIgnoreCase) &&
                c.Model.Equals(model, StringComparison.OrdinalIgnoreCase));

            if (car == null)
                return NotFound();

            return View(car);
        }

        // GET: /Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: /Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string subject, string message)
        {
            // TODO: wire up email sending (e.g. SendGrid, SMTP) here
            // For now, redirect back with a success flag
            TempData["ContactSuccess"] = true;
            return RedirectToAction(nameof(Contact));
        }
    }
}
