
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Xpandables.Net.Aggregates;
using Xpandables.Net.Api.Domains;
using Xpandables.Net.Commands;
using Xpandables.Net.Decorators;
using Xpandables.Net.Expressions;
using Xpandables.Net.Http;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed record Contact(string Id, string Name, string City, string Address, string Country);

    [HttpRestClient(Path = "api/contacts", Method = HttpMethodVerbs.Get, IsSecured = true, IsNullable = true, In = ParameterLocation.Query)]
    public sealed class SelectAllQuery : QueryExpression<ContactAggregate>, IHttpRestClientAsyncRequest<Contact>, IAsyncQuery<Contact>, IQueryStringLocationRequest, ILoggingDecorator
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public override Expression<Func<ContactAggregate, bool>> GetExpression()
        {
            var queryExpression = QueryExpressionFactory.Create<ContactAggregate>();
            if (Name is not null) queryExpression = queryExpression.And(contact => contact.Name.Contains(Name));
            if (City is not null) queryExpression = queryExpression.And(contact => contact.City.Contains(City));
            if (Address is not null) queryExpression = queryExpression.And(contact => contact.Address.Contains(Address));
            if (Country is not null) queryExpression = queryExpression.And(contact => contact.Country.Contains(Country));

            return queryExpression;
        }
        public IDictionary<string, string?>? GetQueryStringSource() => new Dictionary<string, string?>
            {
                {nameof(Name), Name },
                {nameof(City), City },
                {nameof(Address), Address },
                {nameof(Country), Country }
            };
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = HttpMethodVerbs.Get, IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed class SelectQuery : IQuery<Contact>, IHttpRestClientRequest<Contact>, IPathStringLocationRequest, IInterceptorDecorator, ILoggingDecorator
    {
        public SelectQuery(string id)
        {
            Id = id;
        }

        [Required]
        public string Id { get; set; }
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts", Method = "Post", IsSecured = false)]
    public sealed class AddCommand :
        QueryExpression<ContactAggregate>, ICommand<string>, IHttpRestClientRequest<string>, IValidatorDecorator, IAggregate, 
        IPersistenceDecorator, IInterceptorDecorator, ILoggingDecorator
    {
        public AddCommand(string name, string city, string address, string country)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            City = city ?? throw new ArgumentNullException(nameof(city));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }

        [Required] public string Name { get; }
        [Required] public string City { get; }
        [Required] public string Address { get; }
        [Required] public string Country { get; }
        public override Expression<Func<ContactAggregate, bool>> GetExpression() => contact => contact.Name == Name && contact.City == City && contact.Country == Country;
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = HttpMethodVerbs.Delete, IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed class DeleteCommand : ICommand, IHttpRestClientRequest, IValidatorDecorator, IAggregate, 
        IPersistenceDecorator, IPathStringLocationRequest, ILoggingDecorator
    {
        public DeleteCommand(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        [Required]
        public string Id { get; set; }
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts/ip/{id}", Method = HttpMethodVerbs.Get, IsSecured = true, IsNullable = true, In = ParameterLocation.Query | ParameterLocation.Path)]
    public sealed class GetIpQuery : IQuery<IPAddressLocation>, IHttpRestClientRequest<IPAddressLocation>, IPathStringLocationRequest, ILoggingDecorator
    {
        public GetIpQuery(string id)
        {
            Id = id;
        }

        [Required]
        public string Id { get; set; }
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = HttpMethodVerbs.Put, IsSecured = false, IsNullable = false, In = ParameterLocation.Body | ParameterLocation.Path)]
    public sealed class EditCommand :
        ICommand<Contact>, IHttpRestClientRequest<Contact>, IValidatorDecorator, IAggregate, 
        IPersistenceDecorator, ILoggingDecorator, IPathStringLocationRequest, IStringRequest
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public Func<EditCommand, IOperationResult> ApplyPatch = null!;

        [return: NotNull]
        public IDictionary<string, string> GetPathStringSource()
        {
            return new Dictionary<string, string> { { nameof(Id), Id } };
        }

        [return: NotNull]
        public object GetStringContent()
        {
            return new { Id, Name, City, Address, Country };
        }
    }
}
