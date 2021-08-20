
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
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a helper class to implement an identity for an aggregate type.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public class AggregateId : UniqueKey<Guid>, IAggregateId
    {
        /// <summary>
        /// Returns the string representation of the target <see cref="AggregateId"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate to act on.</param>
        public static implicit operator string(AggregateId aggregateId) => aggregateId.Value.ToString();

        /// <summary>
        /// Returns a new instance of <see cref="AggregateId"/> with the specified value.
        /// </summary>
        /// <param name="id">The value of identifier for the new instance.</param>
        /// <returns>A new instance of <see cref="AggregateId"/>.</returns>
        public static AggregateId NewAggregateId(Guid id) => new(id);

        /// <summary>
        /// Returns anew instance of <see cref="AggregateId"/> from the specified string value.
        /// </summary>
        /// <param name="value">The string value to be converted to <see cref="Guid"/>.</param>
        /// <exception cref="ArgumentException">The <paramref name="value"/> can not be converted to <see cref="Guid"/>.</exception>
        /// <returns>A new instance of <see cref="AggregateId"/>.</returns>
        public static AggregateId NewAggregateId(string value) => new(value);

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateId"/> with the value identifier.
        /// </summary>
        /// <param name="value">The value identifier.</param>
        [JsonConstructor]
        public AggregateId(Guid value) : base(value) { }

        /// <summary>
        /// Constructs a new instance of <see cref="AggregateId"/> with the string value identifier.
        /// </summary>
        /// <param name="value">The value identifier.</param>
        /// <exception cref="ArgumentException">The <paramref name="value"/> can not be converted to <see cref="Guid"/> type.</exception>
        public AggregateId(string value)
            : base(Guid.TryParse(value, out var guid)
                  ? guid 
                  : throw new ArgumentException($"The specified value '{value}' can not be converted to '{nameof(Guid)}' type"))
        { }

        ///<inheritdoc/>
        public override bool IsEmpty() => Value == Guid.Empty;

        /// <summary>
        /// Returns the <see cref="string"/> representation of the <see cref="Guid"/> value.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string AsString() => Value.ToString();
    }
}
