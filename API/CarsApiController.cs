using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsedAndReliableCars.Models;

namespace UsedAndReliableCars.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsApiController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = new UsedCar().UsedCars;
            return Ok(cars);
        }


    }
}
