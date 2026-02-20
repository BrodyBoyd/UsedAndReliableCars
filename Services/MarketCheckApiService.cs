using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UsedAndReliableCars.Services;

public class MarketCheckApiService /*: IMarketCheckApiService*/
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

    private async Task<HttpResponseMessage> GetAsync( string path, IReadOnlyDictionary<string, string>? queryParams, CancellationToken cancellationToken )
    {
        var query = BuildQuery(queryParams, _apiKey);
        return await _httpClient.GetAsync(path + query, cancellationToken).ConfigureAwait(false);
    }

    private Task<HttpResponseMessage> GetByIdAsync( string path, string id, CancellationToken cancellationToken )
    {
        var query = BuildQuery(null, _apiKey);
        return _httpClient.GetAsync($"{path}/{Uri.EscapeDataString(id)}{query}", cancellationToken);
    }

    public Task<HttpResponseMessage> SearchFsboActiveAsync( IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default )
       => GetAsync("/v2/search/car/fsbo/active", queryParams, cancellationToken);
}