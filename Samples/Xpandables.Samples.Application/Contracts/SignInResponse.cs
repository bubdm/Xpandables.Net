using Xpandables.Net.HttpRestClient;

namespace Xpandables.Samples.Business.Contracts
{
    public sealed class SignInResponse
    {
        public SignInResponse(string token, string email, GeoLocationResponse iPGeoLocation)
            => (Token, Email, IPGeoLocation) = (token, email, iPGeoLocation);

        public string Token { get; set; }
        public string Email { get; set; }
        public GeoLocationResponse IPGeoLocation { get; set; }
    }
}
