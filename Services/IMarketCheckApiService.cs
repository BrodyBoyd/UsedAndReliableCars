using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UsedAndReliableCars.Services
{
    public interface IMarketCheckApiService
    {
        Task<HttpResponseMessage> SearchActiveAsync(
            IReadOnlyDictionary<string, string>? queryParams = null,
            CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> SearchFsboActiveAsync(
            IReadOnlyDictionary<string, string>? queryParams = null,
            CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> GetHistoryByVinAsync(
            string vin,
            IReadOnlyDictionary<string, string>? queryParams = null,
            CancellationToken cancellationToken = default);
    }
}
