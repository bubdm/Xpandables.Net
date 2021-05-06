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

using Xpandables.Net.Api.Domains.Events;
using Xpandables.Net.Api.Domains.Integrations;
using Xpandables.Net.Api.Domains.Mementos;
using Xpandables.Net.Database;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Api.Domains
{
#pragma warning disable CS8618 
    public sealed class ContactAggregate : Aggregate, IOriginator
    {
        public static string FirstGuidCreated { get; set; } = string.Empty;

        public static ContactAggregate CreateNewContact(string name, string city, string address, string country)
            => new(name, city, address, country);

        public ContactAggregate() : base() { }
        private ContactAggregate(string name, string city, string address, string country)
        {
            RaiseEvent(new ContactCreatedEvent(name, city, address, country, Guid, GetNewVersion()));
        }

        IMemento IOriginator.CreateMemento()
            => new ContactMemento(Guid, Version, Name, City, Address, Country);

        void IOriginator.SetMemento(IMemento memento)
        {
            var contactMemento = (ContactMemento)memento;
            Guid = contactMemento.Id;
            Version = contactMemento.Version;
            Name = contactMemento.Name;
            City = contactMemento.City;
            Address = contactMemento.Address;
            Country = contactMemento.Country;
        }

        public void CancelNameChange(string oldName)
        {
            RaiseEvent(new ContactNameChangeCanceledEvent(oldName, Guid, GetNewVersion()));
        }

        public void ChangeContactName(string name)
        {
            AddIntegrationEvent(new ContactNameChangeIntegrationEvent(name, Name, Guid));
            RaiseEvent(new ContactNameChangedEvent(name, Guid, GetNewVersion()));
        }

        public void ChangeContactCity(string city)
        {
            RaiseEvent(new ContactCityChangedEvent(city, Guid, GetNewVersion()));
        }

        public void ChangeContactAddress(string address)
        {
            RaiseEvent(new ContactAddressChangedEvent(address, Guid, GetNewVersion()));
        }

        public void ChangeContactCountry(string country)
        {
            RaiseEvent(new ContactCountryChangedEvent(country, Guid, GetNewVersion()));
        }

        void On(ContactCreatedEvent createdEvent)
        {
            Name = createdEvent.Name;
            City = createdEvent.City;
            Address = createdEvent.Address;
            Country = createdEvent.Country;
        }

        void On(ContactNameChangeCanceledEvent contactNameChangeCanceledEvent)
        {
            Name = contactNameChangeCanceledEvent.OldName;
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
            RegisterEventHandler<ContactNameChangeCanceledEvent>(On);
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
