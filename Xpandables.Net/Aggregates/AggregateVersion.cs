
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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// The aggregate version.
    /// </summary>
    public sealed class AggregateVersion :
        IEqualityComparer<AggregateVersion>, IEquatable<AggregateVersion>, IComparable<AggregateVersion>
    {
        /// <summary>
        /// Gets the current value of the version.
        /// </summary>
        public long Value { get; } = 0;

        /// <summary>
        /// Constructs a new instance with the specified value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        [JsonConstructor]
        public AggregateVersion(long value) => Value = value;

        ///<inheritdoc/>
        public int CompareTo(AggregateVersion? other)
            => other is null ? -1 : Value.CompareTo(other.Value);

        ///<inheritdoc/>
        public bool Equals(AggregateVersion? other)
            => other is not null && GetType() == other.GetType() && Value.Equals(other.Value);

        ///<inheritdoc/>
        public bool Equals(AggregateVersion? x, AggregateVersion? y) => x?.Equals(y) ?? false;

        ///<inheritdoc/>
        public int GetHashCode([DisallowNull] AggregateVersion obj) => Value.GetHashCode();

        ///<inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as AggregateVersion);

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        ///<inheritdoc/>
        public static bool operator ==(AggregateVersion left, AggregateVersion right)
            => left?.Equals(right) ?? right is null;

        ///<inheritdoc/>
        public static bool operator !=(AggregateVersion left, AggregateVersion right)
            => !(left == right);

        ///<inheritdoc/>
        public static bool operator <(AggregateVersion left, AggregateVersion right)
            => left is null ? !(right is null) : left.CompareTo(right) < 0;

        ///<inheritdoc/>
        public static bool operator <=(AggregateVersion left, AggregateVersion right)
            => left is null || left.CompareTo(right) <= 0;

        ///<inheritdoc/>
        public static bool operator >(AggregateVersion left, AggregateVersion right)
            => !(left is null) && left.CompareTo(right) > 0;

        ///<inheritdoc/>
        public static bool operator >=(AggregateVersion left, AggregateVersion right)
            => left is null ? right is null : left.CompareTo(right) >= 0;

        ///<inheritdoc/>
        public static implicit operator long(AggregateVersion version) => version.Value;

        ///<inheritdoc/>
        public static implicit operator AggregateVersion(long value) => new(value);

        ///<inheritdoc/>
        public static AggregateVersion operator +(AggregateVersion left, AggregateVersion right)
            => new(left.Value + right.Value);

        ///<inheritdoc/>
        public static AggregateVersion operator +(long value, AggregateVersion right)
            => new(value + right.Value);

        ///<inheritdoc/>
        public static AggregateVersion operator -(AggregateVersion left, AggregateVersion right)
            => new(right.Value - left.Value);

        ///<inheritdoc/>
        public static AggregateVersion operator -(int value, AggregateVersion right)
            => new(right.Value - value);
    }
}
