using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

using Xpandables.Net5.HttpRestClient;

namespace Xpandables.Samples.Business.Services
{
    public sealed record HttpIPService(IConfiguration Configuration, IHttpRestClientIPGeoLocationHandler GeoLocationHandler)
    {
        public async Task<IPGeoLocationResponse> GetIPGeoLocationAsync()
        {
            if (Configuration["AccessKey"] is string accessKey)
            {
                var ipAddress = await GeoLocationHandler.GetIPAddressAsync().ConfigureAwait(false);
                var response = await GeoLocationHandler.GetIPGeoLocationAsync(new IPGeoLocationRequest(ipAddress.Result, accessKey)).ConfigureAwait(false);

                return response.IsValid() ? response.Result : default;
            }

            return default;
        }
    }
}
