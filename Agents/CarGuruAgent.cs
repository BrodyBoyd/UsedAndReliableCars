using OpenAI.Chat;
using System.Text.Json;
using UsedAndReliableCars.Models;
using UsedAndReliableCars.Services;

namespace UsedAndReliableCars.Agents
{
    public class CarGuruAgent
    {
        private readonly ChatClient _chatClient;
        private readonly IMarketCheckApiService _marketCheckService;

        public CarGuruAgent(ChatClient chatClient, IMarketCheckApiService marketCheckService)
        {
            _chatClient = chatClient;
            _marketCheckService = marketCheckService;
        }

        public List<UsedCar> usedCars = new List<UsedCar>
        {
            new UsedCar
            {
                Type = "Sedan",
                PriceCategory = 10000,
                Year = "2014-2021",
                Make = "Mazda",
                Model = "Mazda6"
            },
            new UsedCar
            {
                Type = "Sedan",
                PriceCategory = 15000,
                Year = "2014-2019",
                Make = "Toyota",
                Model = "Corolla"
            },
            new UsedCar
            {
                Type = "SUV",
                PriceCategory = 15000,
                Year = "2018-2024",
                Make = "Chevrolet",
                Model = "Equinox"
            },
            new UsedCar
            {
                Type = "Hybrid Sedan",
                PriceCategory = 20000,
                Year = "2020 - present",
                Make = "Toyota",
                Model = "Corolla Hybrid"
            },
            new UsedCar
            {
                Type = "SUV",
                PriceCategory = 20000,
                Year = "2018-2023 ",
                Make = "Subaru",
                Model = "Crosstrek"
            },
            new UsedCar
            {
                Type = "SUV",
                PriceCategory = 20000,
                Year = "2016-2018",
                Make = "Toyota",
                Model = "RAV4 Hybrid"
            },
            new UsedCar
            {
                Type = "SUV",
                PriceCategory = 20000,
                Year = "2014-2019",
                Make = "Toyota",
                Model = "Highlander"
            },
            new UsedCar
            {
                Type = "SUV",
                PriceCategory = 20000,
                Year = "2015-2021",
                Make = "Lexus",
                Model = "NX"
            },
            new UsedCar
            {
                Type = "Sports Car",
                PriceCategory = 20000,
                Year = "2016-2024",
                Make = "Mazda",
                Model = "MX-5 Miata"
            },
            new UsedCar
            {
                Type = "Truck",
                PriceCategory = 25000,
                Year = "2017-present",
                Make = "Honda",
                Model = "Ridgeline"
            }
        };

        public async Task<string> AskAsync(
            string question,
            string? make = null,
            string? model = null,
            string? year = null,
            string? zip = null,
            int? maxPrice = null,
            string? conversationHistory = null)
        {
            try
            {

                var carData = await GetCarDataAsync(make, model, year, zip, maxPrice);

                var messages = new List<OpenAI.Chat.ChatMessage>
                {
                    new SystemChatMessage(
                        $"""
                        You are a helpful car inventory assistant named Car-oline, and you work for a used car dealership called AutoGems.
                        AutoGems is a company that collects data on used and reliable cars and directs customers to the information on the cars.

                        Answer questions only using the real market listings data provided below.
                        You want to give users the best deals on used and reliable cars.
                        Do not invent or assume any details not present in the Car Listings data seen below.
                        You will get an initial message from the user asking for the best deal, please return the best deal but also ask follow up questions to get more information about the user's needs and preferences, such as their budget, desired make/model/year, location, and any must-have features.
                        Car listings (JSON):
                        {carData}
                        Message history:
                        {conversationHistory ?? "No history yet."}
                        """
                    ),
                    new UserChatMessage(question)
                };



                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
                return completion.Content.FirstOrDefault()?.Text ?? "No response.";
            }
            catch (HttpRequestException httpEx)
            {
                return $"Error fetching car data: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                return $"Error generating response: {ex.Message}";
            }
        }

        private async Task<string> GetCarDataAsync(string? make, string? model, string? year, string? zip, int? maxPrice)
        {
            // Build query params for MarketCheck API
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(make)) queryParams["make"] = make;
            if (!string.IsNullOrEmpty(model)) queryParams["model"] = model;
            if (!string.IsNullOrEmpty(year)) queryParams["year"] = year;
            if (!string.IsNullOrEmpty(zip)) queryParams["zip"] = zip;

            // Always cap rows to avoid blowing the context window
            queryParams["rows"] = "20";
            queryParams["start"] = "0";

            if (maxPrice.HasValue && maxPrice.Value > 0)
                queryParams["price_range"] = $"0-{maxPrice.Value}";

            var httpResponse = await _marketCheckService.SearchActiveAsync(queryParams);
            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync();

            // Deserialize into our typed model
            var result = JsonSerializer.Deserialize<MarketCheckSearchResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // If API didn't support price_range or to be extra-safe, filter results locally
            if (maxPrice.HasValue && result?.Listings != null)
            {
                result.Listings = result.Listings
                    .Where(l => !l.Price.HasValue || l.Price.Value <= maxPrice.Value)
                    .ToList();
            }

            if (result?.Listings == null || result.Listings.Count == 0)
                return "No listings found matching your criteria.";

            // Slim down to essentials — photos/URLs etc. waste tokens
            var slim = result.Listings.Select(l => new
            {
                l.Year,
                l.Make,
                l.Model,
                l.Trim,
                l.Price,
                l.Miles,
                l.ExteriorColor,
                l.InteriorColor,
                l.City,
                l.State,
                l.SellerName,
                l.VdpUrl
            });

            return JsonSerializer.Serialize(slim, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}