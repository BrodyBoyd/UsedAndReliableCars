using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UsedAndReliableCars.Services;

public class MarketCheckApiService : IMarketCheckApiService
{
    private const string BaseUrl = "https://api.marketcheck.com";
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public MarketCheckApiService( HttpClient httpClient, IConfiguration configuration )
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _apiKey = configuration["MarketCheck:ApiKey"] ?? "";
    }

    private static string BuildQuery( IReadOnlyDictionary<string, string>? queryParams, string apiKey )
    {
        var pairs = new List<KeyValuePair<string, string>>
        {
            new("api_key", apiKey)
        };
        if (queryParams != null)
            pairs.AddRange(queryParams);
        return "?" + string.Join("&", pairs.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
    }

    private Task<HttpResponseMessage> GetAsync(string path, IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default)
    {
        // Build the query (includes api_key)
        var query = BuildQuery(queryParams, _apiKey);
        // Combine path and query to form the request URI (relative to BaseAddress)
        var requestUri = path + query;
        return _httpClient.GetAsync(requestUri, cancellationToken);
    }

    public Task<HttpResponseMessage> SearchActiveAsync( IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default )
        => GetAsync("/v2/search/car/active", queryParams, cancellationToken);

    public Task<HttpResponseMessage> SearchFsboActiveAsync( IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default )
        => GetAsync("/v2/search/car/fsbo/active", queryParams, cancellationToken);

    public Task<HttpResponseMessage> GetHistoryByVinAsync( string vin, IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default )
        => GetAsync("/v2/history/car/" + Uri.EscapeDataString(vin), queryParams, cancellationToken);
}