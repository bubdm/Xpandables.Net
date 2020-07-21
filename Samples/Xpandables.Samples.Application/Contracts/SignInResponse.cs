using Xpandables.Net.Entities;
using Xpandables.Net.HttpRestClient;

namespace Xpandables.Samples.Business.Contracts
{
    public sealed class SignInResponse
    {
        public SignInResponse(string token, string email, string lastName, string firstName, Gender gender, GeoLocationResponse iPGeoLocation)
            => (Token, Email, FirstName, LastName, Gender, IPGeoLocation) = (token, email, firstName, lastName, gender, iPGeoLocation);

        public string Token { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public GeoLocationResponse IPGeoLocation { get; set; }
    }
}
