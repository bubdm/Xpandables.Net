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

using Xpandables.Net.Api.Models.Events;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Api.Models
{
#pragma warning disable CS8618 
    public sealed class ContactModel : AggregateRoot
    {
        public static string FirstGuidCreated { get; set; } = string.Empty;

        public static ContactModel CreateNewContact(string name, string city, string address, string country)
            => new(name, city, address, country);

        public ContactModel() : base() { }
        private ContactModel(string name, string city, string address, string country)
        {
            Apply(new ContactCreatedEvent(name, city, address, country, Guid, GetNewVersion()));
        }

        public void ChangeContactName(string name)
        {
            Apply(new ContactNameChangedEvent(name, Guid, GetNewVersion()));
        }

        public void ChangeContactCity(string city)
        {
            Apply(new ContactCityChangedEvent(city, Guid, GetNewVersion()));
        }

        public void ChangeContactAddress(string address)
        {
            Apply(new ContactAddressChangedEvent(address, Guid, GetNewVersion()));
        }

        public void ChangeContactCountry(string country)
        {
            Apply(new ContactCountryChangedEvent(country, Guid, GetNewVersion()));
        }

        void On(ContactCreatedEvent createdEvent)
        {
            Name = createdEvent.Name;
            City = createdEvent.City;
            Address = createdEvent.Address;
            Country = createdEvent.Country;
        }

        void On(ContactNameChangedEvent nameChangedEvent)
        {
            Name = nameChangedEvent.Name;
        }

        void On(ContactCityChangedEvent cityChangedEvent)
        {
            City = cityChangedEvent.City;
        }

        void On(ContactAddressChangedEvent addressChangedEvent)
        {
            Address = addressChangedEvent.Address;
        }

        void On(ContactCountryChangedEvent countryChangedEvent)
        {
            Country = countryChangedEvent.Country;
        }

        protected override void RegisterEventHandlers()
        {
            RegisterEventHandler<ContactCreatedEvent>(On);
            RegisterEventHandler<ContactNameChangedEvent>(On);
            RegisterEventHandler<ContactAddressChangedEvent>(On);
            RegisterEventHandler<ContactCityChangedEvent>(On);
            RegisterEventHandler<ContactCountryChangedEvent>(On);
        }

        public string Name { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }
        public string Country { get; private set; }
    }
}
