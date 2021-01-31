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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Api.Models
{
    public sealed record ContactModelCreatedDomainEvent(string Id) : IDomainEvent { public DateTime OccurredOn => DateTime.UtcNow; }
    public sealed record ContactModelCreatedIntegrationEvent(string Id) : IIntegrationEvent { public DateTime OccurredOn => DateTime.UtcNow; }
    public sealed record ContactModelUpdatedDomainEvent(string Id, string? Name) : IDomainEvent { public DateTime OccurredOn => DateTime.UtcNow; };

    public sealed class ContactModel : Entity, IAggregateRoot
    {
        public ContactModel(string name, string city, string address, string country)
        {
            Update(name, city, address, country);
            AddNotification(new ContactModelCreatedDomainEvent(Id));
            AddNotification(new ContactModelCreatedIntegrationEvent(Id));
        }

        [MemberNotNull(nameof(Name), nameof(City), nameof(Address), nameof(Country))]
        public void Update(string name, string city, string address, string country)
        {
            UpdateName(name);
            UpdateCity(city);
            UpdateAddress(address);
            UpdateCountry(country);
        }
        public void Edit(string? name, string? city, string? address, string? country)
        {
            if (name is not null) UpdateName(name);
            if (city is not null) UpdateCity(city);
            if (address is not null) UpdateAddress(address);
            if (country is not null) UpdateCountry(country);
            AddNotification(new ContactModelUpdatedDomainEvent(Id, name));
        }

        [MemberNotNull(nameof(Name))]
        public void UpdateName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
        [MemberNotNull(nameof(City))]
        public void UpdateCity(string city) => City = city ?? throw new ArgumentNullException(nameof(city));
        [MemberNotNull(nameof(Address))]
        public void UpdateAddress(string address) => Address = address ?? throw new ArgumentNullException(nameof(address));
        [MemberNotNull(nameof(Country))]
        public void UpdateCountry(string country) => Country = country ?? throw new ArgumentNullException(nameof(country));
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }
        public string Country { get; private set; }
        protected override string KeyGenerator()
        {
            var stringBuilder = new StringBuilder();

            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(_ => Guid.NewGuid())
                .Take(32)
                .ToList()
                .ForEach(e => stringBuilder.Append(e));

            return stringBuilder.ToString().ToUpperInvariant();
        }
    }
}
