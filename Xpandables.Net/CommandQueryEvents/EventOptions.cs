
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
using Xpandables.Net.Decorators;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure event options.
    /// </summary>
    public sealed class EventOptions
    {
        /// <summary>
        /// Enables persistence behavior to events that are decorated with the <see cref="IPersistenceDecorator"/> .
        /// </summary>
        public EventOptions UsePersistenceDecorator() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables persistence behavior to events that are decorated with the <see cref="IAggregatePersistenceDecorator"/> .
        /// </summary>
        public EventOptions UseAggregatePersistenceDecorator() => this.With(cq => cq.IsAggregatePersistenceEnabled = true);

        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsAggregatePersistenceEnabled { get; private set; }
    }
}
