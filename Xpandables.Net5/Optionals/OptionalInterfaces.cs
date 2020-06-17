
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System
{
    public readonly partial struct Optional<T> :
        IEquatable<Optional<T>>, IEquatable<T>, IComparable<Optional<T>>, IComparable<T>, IFormattable
    {
        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///   Value
        ///   Meaning
        ///   Less than zero
        ///   This instance precedes <paramref name="other" /> in the sort order.
        ///   Zero
        ///   This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///   Greater than zero
        ///   This instance follows <paramref name="other" /> in the sort order.</returns>
        public int CompareTo(Optional<T> other)
            => IsValue() && other.IsValue()
                ? Comparer<T>.Default.Compare(_values[0], other._values[0])
                : IsValue() && !other.IsValue() ? 1 : -1;

        /// <summary>Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///   Value
        ///   Meaning
        ///   Less than zero
        ///   This instance precedes <paramref name="other" /> in the sort order.
        ///   Zero
        ///   This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///   Greater than zero
        ///   This instance follows <paramref name="other" /> in the sort order.</returns>
        public int CompareTo([AllowNull] T other)
            => IsValue() && (other is null)
                ? 1
                : !IsValue() && (other is { })
                    ? -1
                    : IsValue() && (other is { })
                        ? Comparer<T>.Default.Compare(_values[0], other)
                        : 0;

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Optional<T> other)
            => IsValue() && other.IsValue() ? _values[0]!.Equals(other._values[0]) : !IsValue() && !other.IsValue();

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals([AllowNull] T other) => IsValue() && other is { };

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value;
        ///   otherwise, <see langword="false" />.</returns>
        public readonly override bool Equals(object? obj) => obj is Optional<T> optional && Equals(optional);

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public readonly override int GetHashCode()
        {
            const int hash = 17;
            if (IsValue())
                return _values[0]!.GetHashCode() ^ 31;
            return hash ^ 29;
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public readonly override string ToString() => IsValue() ? $"{_values[0]}" : string.Empty;

        /// <summary>Formats the value of the current instance using the specified format.</summary>
        /// <param name="format">The format to use.
        ///  -or-
        ///  A null reference (<see langword="Nothing" /> in Visual Basic) to use the default format defined
        ///  for the type of the <see cref="IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.
        ///  -or-
        ///  A null reference (<see langword="Nothing" /> in Visual Basic) to obtain the numeric
        ///  format information from the current locale setting of the operating system.</param>
        /// <returns>The value of the current instance in the specified format.</returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
            => IsValue() ? string.Format(formatProvider, "{0:" + format + "}", _values[0]) : string.Empty;

#pragma warning disable CS1591 // Comment
        public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);

        public static bool operator !=(Optional<T> left, Optional<T> right) => !(left == right);

        public static bool operator <(Optional<T> left, Optional<T> right) => left.CompareTo(right) < 0;

        public static bool operator <=(Optional<T> left, Optional<T> right) => left.CompareTo(right) <= 0;

        public static bool operator >(Optional<T> left, Optional<T> right) => left.CompareTo(right) > 0;

        public static bool operator >=(Optional<T> left, Optional<T> right) => left.CompareTo(right) >= 0;

        public static implicit operator Optional<T>([AllowNull] T result) => result is { } ? Some(result) : Empty();

        public static implicit operator Optional<T>(Optional<Optional<T>> optional) => optional._values[0];
#pragma warning restore CS1591 // Comment
    }
}
