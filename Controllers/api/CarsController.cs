using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace UsedAndReliableCars.Controllers.api
{
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public int PageSize { get; set; } = 6;

        public CarsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.marketcheck.com/v2/");
            _configuration = configuration;
        }

        //Can add more or remove params
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? make, string? model, int? year, decimal? priceMax, string? sortBy, string? type, int? PageNum = 0)
        {
            var apiKey = _configuration["MarketCheck:ApiKey"];

            var url = $"search/car/fsbo/active?api_key={apiKey}";
            if (!string.IsNullOrEmpty(make))
                url += $"&make={Uri.EscapeDataString(make)}";

            if (!string.IsNullOrEmpty(model))
                url += $"&model={Uri.EscapeDataString(model)}";

            if (year.HasValue) 
                url += $"&year={year.Value}";

            if (priceMax.HasValue)
                url += $"&price_range=0-{priceMax.Value}";

            //Console.WriteLine(sortBy);

            //if (!string.IsNullOrEmpty(sortBy))
            //{
            //    if (sortBy == "price")
            //    {
            //        url += $"&sort_by=price";
            //    }
            //    else if (sortBy == "miles")
            //    {
            //        url += $"&sort_by=miles";
            //    }
            //} else
            //{
            //    url += $"&sort_by=price";
            //}
            
            url += $"&rows={PageSize}";
            url += $"&start={PageNum * PageSize}";




            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                   return StatusCode(503, new { error = "Error fetching data from MarketCheck API" });
            }


            var json = await response.Content.ReadAsStringAsync();
            if (!string.Equals(type, "hybrid", StringComparison.OrdinalIgnoreCase))
            {
                return Content(json, "application/json");
            }

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            var filteredListings = new List<JsonElement>();

            foreach (var listing in root.GetProperty("listings").EnumerateArray())
            {
                var heading = listing.TryGetProperty("heading", out var h) ? h.GetString() : null;

                var version = listing.TryGetProperty("build", out var build) &&
                              build.TryGetProperty("version", out var v)
                                ? v.GetString()
                                : null;

                bool isHybrid =
                 (heading?.Contains("Hybrid", StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (version?.Contains("Hybrid", StringComparison.OrdinalIgnoreCase) ?? false);

                if (isHybrid)
                    filteredListings.Add(listing);
            }

            var resultObject = new
            {
                ResultCount = filteredListings.Count,
                listings = filteredListings
            };

            var filtered = filteredListings
                .Select(l => JsonSerializer.Deserialize<object>(l.GetRawText()))
                .ToList();

            return Ok(new
            {
                ResultCount = filtered.Count,
                listings = filtered
            });

        }
    }
}
