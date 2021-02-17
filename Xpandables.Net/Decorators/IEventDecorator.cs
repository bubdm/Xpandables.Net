﻿/************************************************************************************************************
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

using Xpandables.Net.Events.DomainEvents;
using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Decorators
{
    /// <summary>
    /// A marker interface that allows the command class to add domain event decorator on the control flow before changes are committed.
    /// You need to provides implementation of <see cref="IIntegrationEventHandler{TEvent}"/> and/or <see cref="IDomainEventHandler{TEvent}"/>.
    /// </summary>
    public interface IEventDecorator { }
}