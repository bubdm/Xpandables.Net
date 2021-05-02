﻿
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

using Xpandables.Net.Events.DomainEvents;

namespace Xpandables.Net.Api.Models.Events
{
    public class ContactCreatedEvent : DomainEvent
    {
        public string Name { get; }
        public string City { get; }
        public string Address { get; }
        public string Country { get; }
        public ContactCreatedEvent(string name, string city, string address, string country, Guid aggregateId, long version)
            : base(aggregateId, version)
        {
            Name = name;
            City = city;
            Address = address;
            Country = country;
        }
    }
}