
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
using Xpandables.Net.Database;

namespace Xpandables.Net.Events.DomainEvents
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as a event domain.
    /// This kind of events are published before <see cref="IDataContext.PersistAsync(System.Threading.CancellationToken)"/>.
    /// In case of exception in target event handlers, you can rollback the operation using transaction.
    /// </summary>
    public interface IDomainEvent : IEvent { }
}
