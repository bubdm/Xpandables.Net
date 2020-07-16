using Xpandables.Net5.Entities;
using Xpandables.Net5.HttpRestClient;

namespace Xpandables.Samples.Business.Contracts
{
    public sealed class SignInResponse
    {
        public SignInResponse(string token, string email, string lastName, string firstName, Gender gender, IPGeoLocationResponse iPGeoLocation)
            => (Token, Email, FirstName, LastName, Gender, IPGeoLocation) = (token, email, firstName, lastName, gender, iPGeoLocation);

        public string Token { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public IPGeoLocationResponse IPGeoLocation { get; set; }
    }
}
