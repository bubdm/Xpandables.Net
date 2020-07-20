using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

using Xpandables.Net5.HttpRestClient;

namespace Xpandables.Samples.Business.Services
{
    public sealed class HttpIPService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpRestClientIPGeoLocationHandler _geoLocationHandler;
        public HttpIPService(IConfiguration Configuration, IHttpRestClientIPGeoLocationHandler GeoLocationHandler)
        {
            _configuration = Configuration;
            _geoLocationHandler = GeoLocationHandler;
        }

        public async Task<IPGeoLocationResponse> GetIPGeoLocationAsync()
        {
            if (_configuration["AccessKey"] is string accessKey)
            {
                var ipAddress = await _geoLocationHandler.GetIPAddressAsync().ConfigureAwait(false);
                var response = await _geoLocationHandler.GetIPGeoLocationAsync(new IPGeoLocationRequest(ipAddress.Result, accessKey)).ConfigureAwait(false);

                return response.IsValid() ? response.Result : default;
            }

            return default;
        }
    }
}
