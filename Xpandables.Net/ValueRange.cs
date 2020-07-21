
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
using System.ComponentModel;
using System.Diagnostics;

namespace Xpandables.Net
{
    /// <summary>
    /// Defines a pair of values, representing a segment.
    /// This class uses <see cref="ValueRangeConverter"/> as type converter.
    /// </summary>
    /// <typeparam name="TValue">The Type of each of two values of range.</typeparam>
    [Serializable]
    [DebuggerDisplay("Min = {Min} : Max = {Max}")]
    [TypeConverter(typeof(ValueRangeConverter))]
    public sealed class ValueRange<TValue>
        where TValue : unmanaged, IComparable, IFormattable, IConvertible, IComparable<TValue>, IEquatable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValueRange{TValue}"/> with the specified values.
        /// </summary>
        /// <param name="min">The minimal value of range.</param>
        /// <param name="max">The maximal value of range.</param>
        public ValueRange(TValue min, TValue max) => (Min, Max) = (min, max);

        /// <summary>
        /// Gets the minimal value of range.
        /// </summary>
        public TValue Min { get; private set; }

        /// <summary>
        /// Gets the maximal value of range.
        /// </summary>
        public TValue Max { get; private set; }

        /// <summary>
        /// Creates a string representation of the <see cref="ValueRange{T}"/> separated by ":".
        /// </summary>
        public override string ToString() => $"{Min}:{Max}";

        /// <summary>
        /// Creates a string representation of the string <see cref="ValueRange{TValue}"/> using the specified format and provider.
        /// The format will received address properties in the following order : <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public string ToString(string format, IFormatProvider formatProvider) => string.Format(formatProvider, format, Min, Max);

        /// <summary>
        /// Compares the <see cref="ValueRange{TValue}"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object? obj) => obj is ValueRange<TValue> signedValues && this == signedValues;

        /// <summary>
        /// Computes the hash-code for the <see cref="ValueRange{TValue}"/> instance.
        /// </summary>
        public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(ValueRange<TValue> left, ValueRange<TValue> right)
            => left is null && right is null || !(left is null) && !(right is null) && left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(ValueRange<TValue> left, ValueRange<TValue> right) => !(left == right);

        /// <summary>
        /// Compares <see cref="ValueRange{TValue}"/> with the value of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(ValueRange<TValue> other) => other is ValueRange<TValue> && Min.Equals(other.Min) && Max.Equals(other.Max);

        /// <summary>
        /// Determines whether this range is empty or not.
        /// Returns <see langword="true"/> if so, otherwise returns <see langword="false"/>.
        /// </summary>
        public bool IsEmpty() => Min.CompareTo(Max) >= 0;
    }
}
