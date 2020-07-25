using Microsoft.EntityFrameworkCore;

using System;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Cryptography;
using Xpandables.Net.Data;
using Xpandables.Net.Data.Attributes;
using Xpandables.Net.Data.Elements;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Helpers;
using Xpandables.Net.Http;
using Xpandables.Net.Queries;
using Xpandables.Samples.Business.Contracts;
using Xpandables.Samples.Business.Localization;
using Xpandables.Samples.Business.Services;

namespace Xpandables.Samples.Business.Handlers
{
    public sealed class SignInRequestHandler : IQueryHandler<SignInRequest, SignInResponse>
    {
        private readonly IDataContext _dataContext;
        private readonly IHttpTokenEngine _tokenEngine;
        private readonly HttpIPService _httpIPService;
        private readonly IStringCryptography _stringCryptography;
        private readonly IDataBase _dataBase;

        public SignInRequestHandler(IDataContext dataContext, IDataBase dataBase, IHttpTokenEngine tokenEngine, HttpIPService httpIPService, IStringCryptography stringCryptography)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tokenEngine = tokenEngine ?? throw new ArgumentNullException(nameof(tokenEngine));
            _httpIPService = httpIPService ?? throw new ArgumentNullException(nameof(httpIPService));
            _stringCryptography = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));
            _dataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        }

        public async Task<SignInResponse> HandleAsync(SignInRequest query, CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.SetOf(query).FirstOrDefaultAsync(query, cancellationToken).ConfigureAwait(false);

            var options = new DataOptionsBuilder()
                .AddConverter<string>((property, row) =>
                {
                    if (property.PropertyName == "Gender")
                        return "Unknown";

                    return row;
                })
                .Build();

            var xusers = await _dataBase.ExecuteQueriesAsync<XUser>(options, "Select * from users", cancellationToken).ConfigureAwait(false);

            var options1 = new DataOptionsBuilder()
                .Build();

            var count = await _dataBase.ExecuteTransactionAsync(options1, "update users set name_lastname='last name' where email=@email", cancellationToken, query.Email).ConfigureAwait(false);

            var xusers1 = await _dataBase.ExecuteQueriesAsync<XUser>(options, "Select * from users", cancellationToken).ConfigureAwait(false);

            if (user?.Password.IsEqualTo(query.Password, _stringCryptography) != true)
            {
                throw this.GetValidationException(
                    ErrorMessages.UserNotExist.StringFormat(query.Email),
                    query.Email,
                    nameof(query.Email), nameof(query.Password));
            }

            var token = user.GetToken(_tokenEngine);
            var location = await _httpIPService.GetIPGeoLocationAsync().ConfigureAwait(false);

            return new SignInResponse(
                token, user.Email, user.Name.LastName, user.Name.FirstName, user.Gender, location);
        }

        public class XUser
        {
            public string Email { get; set; }

            [DataNotMapped]
            public ValueEncrypted Password { get; set; }

            public XName Name { get; set; }

            [DataConverter(typeof(Tester), nameof(Tester.Converter))]
            public string Gender { get; set; }
        }

        public class Tester
        {
            public static object Converter(IDataProperty property, object rowValue)
            {
                if (property.PropertyName == "Gender")
                    return "Unknown";

                return rowValue;
            }
        }

        public class XName
        {
            [DataName("Name_LastName")]
            public string LastName { get; set; }
            [DataName("Name_FirstName")]
            public string FirstName { get; set; }
        }
    }
}
