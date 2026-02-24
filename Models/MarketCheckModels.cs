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

        /// <summary>Nested media object returned by the MarketCheck API.</summary>
        [JsonPropertyName("media")]
        public MediaInfo? Media { get; set; }

        /// <summary>
        /// First real photo URL (cached preferred, raw fallback).
        /// Returns null if only placeholder/no-photo images are available,
        /// so the view shows the car emoji fallback instead.
        /// </summary>
        [JsonIgnore]
        public string? FirstPhoto =>
            Media?.PhotoLinksCached?.FirstOrDefault(IsRealPhoto)
            ?? Media?.PhotoLinks?.FirstOrDefault(IsRealPhoto);

        /// <summary>
        /// Returns false for known dealer "no photo available" placeholder images.
        /// Add new patterns here as you spot them in the wild.
        /// </summary>
        private static bool IsRealPhoto(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            string[] placeholderPatterns =
            [
                "nophoto",
                "no_photo",
                "no-photo",
                "noimage",
                "no_image",
                "no-image",
                "newarrivalphoto",              // imagescdn.dealercarsearch.com
                "notavailable",
                "not-available",
                "vehicle-image-notavailable",   // carforniala.com
                "coming-soon",
                "comingsoon",
                "defaultcar",
                "default_car",
                "placeholder",
                "stockphoto",
                "stock_photo",
                "stock-photo",
            ];

            var lower = url.ToLowerInvariant();
            return !Array.Exists(placeholderPatterns, p => lower.Contains(p));
        }
    }

    /// <summary>Photo URLs nested inside a CarListing's "media" field.</summary>
    public class MediaInfo
    {
        /// <summary>MarketCheck-cached copies — preferred; more reliable than dealer-hosted URLs.</summary>
        [JsonPropertyName("photo_links_cached")]
        public List<string>? PhotoLinksCached { get; set; }

        /// <summary>Original dealer-hosted photo URLs.</summary>
        [JsonPropertyName("photo_links")]
        public List<string>? PhotoLinks { get; set; }
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
