
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
using System.Linq.Expressions;

using Xpandables.Net.Api.Models;
using Xpandables.Net.CQRS;
using Xpandables.Net.Expressions.Records;
using Xpandables.Net.Http;
using Xpandables.Net.Interception;

namespace Xpandables.Net.Api.Handlers
{
    public sealed record Contact(string Id, string Name, string City, string Address, string Country);

    [HttpRestClient(Path = "api/contacts", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Query)]
    public sealed record SelectAll : RecordExpression<ContactModel>, IAsyncQuery<Contact>, IQueryStringLocationRequest, ILoggingDecorator
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public override Expression<Func<ContactModel, bool>> GetExpression()
        {
            var queryExpression = RecordExpressionFactory.Create<ContactModel>();
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

    [HttpRestClient(Path = "api/contacts/{id}", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed record Select([Required] string Id) : RecordExpression<ContactModel>, IQuery<Contact>, IPathStringLocationRequest, IInterceptorDecorator, ILoggingDecorator
    {
        public override Expression<Func<ContactModel, bool>> GetExpression() => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts", Method = "Post", IsSecured = false)]
    public sealed record Add([Required] string Name, [Required] string City, [Required] string Address, [Required] string Country)
        : RecordExpression<ContactModel>, ICommand<string>, IValidationDecorator, IPersistenceDecorator, IInterceptorDecorator, IDomainEventDecorator, IIntegrationEventDecorator, ILoggingDecorator
    {
        public override Expression<Func<ContactModel, bool>> GetExpression() => contact => contact.Name == Name && contact.City == City && contact.Country == Country;
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = "Delete", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed record Delete([Required] string Id) : RecordExpression<ContactModel>, ICommand, IValidationDecorator, IPersistenceDecorator, IPathStringLocationRequest, ILoggingDecorator
    {
        public override Expression<Func<ContactModel, bool>> GetExpression() => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = "Post", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed record GetIp([Required] string Id) : IQuery<GeoLocation>, IPathStringLocationRequest, ILoggingDecorator
    {
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), Id } };
    }

    [HttpRestClient(Path = "api/contacts", Method = "Patch", IsSecured = true, IsNullable = false, In = ParameterLocation.Body)]
    public sealed record Edit : RecordExpression<ContactModel>, ICommand<Contact>, IValidationDecorator, IPersistenceDecorator, IDomainEventDecorator, ILoggingDecorator
    {
        public override Expression<Func<ContactModel, bool>> GetExpression() => contact => contact.Id == Id && contact.IsActive && !contact.IsDeleted;
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public Func<Edit, IOperationResult> ApplyPatch = null!;
    }
}
