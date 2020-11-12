
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
    /// This class uses <see cref="ValueRangeTypeConverter"/> as type converter.
    /// </summary>
    /// <typeparam name="TValue">The Type of each of two values of range.</typeparam>
    [Serializable]
    [DebuggerDisplay("Min = {Min} : Max = {Max}")]
    [TypeConverter(typeof(ValueRangeTypeConverter))]
    public sealed record ValueRange<TValue>
        where TValue : unmanaged, IComparable, IFormattable, IConvertible, IComparable<TValue>, IEquatable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValueRange{TValue}"/> with the specified pair of values.
        /// </summary>
        /// <param name="min">The minimal value of range.</param>
        /// <param name="max">The maximal value of range.</param>
        public ValueRange(TValue min, TValue max) => (Min, Max) = (min, max);

        /// <summary>
        /// Creates a copy of the specified range value.
        /// </summary>
        /// <param name="source">The range to be copied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public ValueRange(ValueRange<TValue> source) => (Min, Max) = (source.Min, source.Max);

        /// <summary>
        /// Provides with deconstruction for <see cref="ValueRange{TValue}"/>.
        /// </summary>
        /// <param name="min">The output minimal value of range.</param>
        /// <param name="max">The output maximal value of range.</param>
        public void Deconstruct(out TValue min, out TValue max) => (min, max) = (Min, Max);

        /// <summary>
        /// Gets the minimal value of range.
        /// </summary>
        public TValue Min { get; }

        /// <summary>
        /// Gets the maximal value of range.
        /// </summary>
        public TValue Max { get; }

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
        /// Determines whether this range is empty or not.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if so, otherwise returns <see langword="false"/>.</returns>
        public bool IsEmpty() => Min.CompareTo(Max) >= 0;
    }
}
