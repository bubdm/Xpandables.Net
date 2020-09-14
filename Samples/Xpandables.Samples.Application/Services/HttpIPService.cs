using Microsoft.Extensions.Configuration;

using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Enumerables;
using Xpandables.Net.HttpRestClient;

namespace Xpandables.Samples.Business.Services
{
    public sealed class HttpIPService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpRestClientIPLocationHandler _ipLocationHandler;
        private readonly IHttpRestClientGeoLocationHandler _geoLocationHandler;
        public HttpIPService(
            IConfiguration Configuration, IHttpRestClientIPLocationHandler GeoLocationHandler, IHttpRestClientGeoLocationHandler geoLocationHandler)
        {
            _configuration = Configuration;
            _ipLocationHandler = GeoLocationHandler;
            _geoLocationHandler = geoLocationHandler;
        }

        public async Task<GeoLocationResponse> GetIPGeoLocationAsync()
        {
            if (_configuration["AccessKey"] is string accessKey)
            {
                var ipAddress = await _ipLocationHandler.GetIPAddressAsync().ConfigureAwait(false);
                var response = await _geoLocationHandler.GetGeoLocationAsync(new GeoLocationRequest((await ipAddress.Result.FirstOrEmptyAsync().ConfigureAwait(false)).Single(), accessKey)).ConfigureAwait(false);

                return response.IsValid() ? (await response.Result.FirstOrEmptyAsync().ConfigureAwait(false)).Single() : default;
            }

            return default;
        }
    }
}
