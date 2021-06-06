
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

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a helper class to implement an identity for an aggregate type.
    /// </summary>
    public abstract class AggregateId : IdentityId<Guid>, IAggregateId
    {
        /// <summary>
        /// Returns the string representation of the target <see cref="AggregateId"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate to act on.</param>
        public static implicit operator string(AggregateId aggregateId) => aggregateId.Value.ToString();

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateId"/> with the value identifier.
        /// </summary>
        /// <param name="value">The value identifier.</param>
        protected AggregateId(Guid value) : base(value) { }
        
        ///<inheritdoc/>
        public override bool IsEmpty() => Value == Guid.Empty;
    }
}
