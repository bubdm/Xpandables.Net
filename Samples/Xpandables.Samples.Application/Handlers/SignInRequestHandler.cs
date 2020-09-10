﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Cryptography;
using Xpandables.Net.Data;
using Xpandables.Net.Data.Attributes;
using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Elements;
using Xpandables.Net.Data.Options;
using Xpandables.Net.Data.Providers;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Enumerables;
using Xpandables.Net.Http;
using Xpandables.Net.Interception;
using Xpandables.Net.Queries;
using Xpandables.Net.Strings;
using Xpandables.Samples.Business.Contracts;
using Xpandables.Samples.Business.Localization;
using Xpandables.Samples.Business.Services;

namespace Xpandables.Samples.Business.Handlers
{
    public sealed class SignInRequestHandler : IAsyncQueryHandler<SignInRequest, SignInResponse>, IInterceptorDecorator
    {
        private readonly IDataContext _dataContext;
        private readonly IHttpTokenEngine _tokenEngine;
        private readonly HttpIPService _httpIPService;
        private readonly IStringCryptography _stringCryptography;
        private readonly IDataBase _dataBase;

        public SignInRequestHandler(IDataBase dataBase, IDataContext dataContext, IHttpTokenEngine tokenEngine, HttpIPService httpIPService, IStringCryptography stringCryptography)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tokenEngine = tokenEngine ?? throw new ArgumentNullException(nameof(tokenEngine));
            _httpIPService = httpIPService ?? throw new ArgumentNullException(nameof(httpIPService));
            _stringCryptography = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));
            _dataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        }

        public async IAsyncEnumerable<SignInResponse> HandleAsync(SignInRequest query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.SetOf(query).FirstOrDefaultAsync(query, cancellationToken).ConfigureAwait(false);

            //var options = new DataOptionsBuilder()
            //    .AddConverter<string>((property, row) =>
            //    {
            //        if (property.PropertyName == "Gender")
            //            return "Unknown";

            //        return row;
            //    })
            //    .Build();

            var options = new DataOptionsBuilder()
                .BuildDefault();

            var connection = new DataConnectionBuilder()
                .AddConnectionString("Server=(localdb)\\mssqllocaldb;Database=XSamples;Trusted_Connection=True;MultipleActiveResultSets=true")
                .AddPoolName("LocalDb")
                .AddProviderType(DataProviderType.MSSQL)
                .EnableIntegratedSecurity()
                .Build();

            var xusers = await _dataBase
                .UseConnection(connection)
                .ExecuteMappedQueriesAsync<XUser>(options, "Select * fromusers")
                .ToListAsync()
                .ConfigureAwait(false);

            //var options1 = new DataOptionsBuilder()
            //    .Build();

            //var count = await _dataBase
            //    .UseConnection(connection)
            //    .ExecuteTransactionAsync(options1, "update users set name_lastname='last name new' where email=@email", query.Email)
            //    .ConfigureAwait(false);

            //var xusers1 = await _dataBase
            //    .UseConnection(connection)
            //    .ExecuteMappedQueriesAsync<XUser>(options, "Select * from users")
            //    .ToListAsync()
            //    .ConfigureAwait(false);

            if (user?.Password.IsEqualTo(query.Password, _stringCryptography) != true)
            {
                throw this.GetValidationException(
                    ErrorMessages.UserNotExist.StringFormat(query.Email),
                    query.Email,
                    nameof(query.Email), nameof(query.Password));
            }

            var token = user.GetToken(_tokenEngine);
            var location = await _httpIPService.GetIPGeoLocationAsync().ConfigureAwait(false);

            yield return new SignInResponse(token, user.Email, location);
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public class XUser
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public string Email { get; set; }

            [DataNotMapped]
            public ValueEncrypted Password { get; set; }
        }
    }
}
