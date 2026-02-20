using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsedAndReliableCars.Models
{
    // -------------------------------------------------------------------------
    // MarketCheck API response — returned by /v2/search/car/active
    // -------------------------------------------------------------------------

    public class MarketCheckSearchResponse
    {
        /// <summary>Total number of listings available server-side.</summary>
        [JsonPropertyName("num_found")]
        public int NumFound { get; set; }

        /// <summary>The page of listings returned in this response.</summary>
        [JsonPropertyName("listings")]
        public List<CarListing> Listings { get; set; } = new();
    }

    /// <summary>A single active car listing from MarketCheck.</summary>
    public class CarListing
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("vin")]
        public string? Vin { get; set; }

        [JsonPropertyName("heading")]
        public string? Heading { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("miles")]
        public int? Miles { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("trim")]
        public string? Trim { get; set; }

        [JsonPropertyName("exterior_color")]
        public string? ExteriorColor { get; set; }

        [JsonPropertyName("interior_color")]
        public string? InteriorColor { get; set; }

        [JsonPropertyName("seller_name")]
        public string? SellerName { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [JsonPropertyName("vdp_url")]
        public string? VdpUrl { get; set; }

        [JsonPropertyName("photo_links")]
        public List<string>? PhotoLinks { get; set; }

        /// <summary>Convenience: first photo URL, or null.</summary>
        [JsonIgnore]
        public string? FirstPhoto => PhotoLinks?.Count > 0 ? PhotoLinks[0] : null;
    }

    // -------------------------------------------------------------------------
    // MarketCheck VIN history — returned by /v2/history/car/{vin}
    // -------------------------------------------------------------------------

    /// <summary>One historical listing record for a given VIN.</summary>
    public class VinHistoryRecord
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("miles")]
        public int? Miles { get; set; }

        [JsonPropertyName("heading")]
        public string? Heading { get; set; }

        [JsonPropertyName("seller_name")]
        public string? SellerName { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        /// <summary>Unix timestamp (seconds) when the listing was first seen.</summary>
        [JsonPropertyName("first_seen_at")]
        public long? FirstSeenAt { get; set; }

        /// <summary>Unix timestamp (seconds) when the listing was last seen.</summary>
        [JsonPropertyName("last_seen_at")]
        public long? LastSeenAt { get; set; }

        /// <summary>Human-readable date derived from FirstSeenAt.</summary>
        [JsonIgnore]
        public string? FirstSeenDate => FirstSeenAt.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(FirstSeenAt.Value).LocalDateTime.ToShortDateString()
            : null;
    }

    // -------------------------------------------------------------------------
    // View-models used by HomeController views
    // -------------------------------------------------------------------------

    /// <summary>View-model for the SearchResults view (FindCars action).</summary>
    public class CarSearchResultViewModel
    {
        public List<CarListing> Listings { get; set; } = new();
        public int TotalFound { get; set; }
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Keyed by VIN. Value is "lower" or "higher" relative to historical
        /// average, set by EnrichPriceTrendsAsync. Missing key = fair/unknown.
        /// </summary>
        public Dictionary<string, string> PriceTrendByVin { get; set; } = new();
    }

    /// <summary>View-model for the PriceHistory view.</summary>
    public class PriceHistoryViewModel
    {
        public string? Vin { get; set; }
        public string? VehicleTitle { get; set; }
        public string? ErrorMessage { get; set; }
        public List<VinHistoryRecord> Records { get; set; } = new();
    }
}
