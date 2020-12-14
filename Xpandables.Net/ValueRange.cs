
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
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
    /// <summary>
    /// Defines a pair of values, representing a segment.
    /// This class uses <see cref="ValueRangeTypeConverter"/> as type converter.
    /// <para>Returns a new instance of <see cref="ValueRange{TValue}"/> with the specified pair of values.</para>
    /// </summary>
    /// <param name="Min">The minimal value of range.</param>
    /// <param name="Max">The maximal value of range.</param>
    /// <typeparam name="TValue">The Type of each of two values of range.</typeparam>
    [Serializable]
    [DebuggerDisplay("Min = {Min} : Max = {Max}")]
    [TypeConverter(typeof(ValueRangeTypeConverter))]
    public sealed record ValueRange<TValue>(TValue Min, TValue Max)
        where TValue : unmanaged, IComparable, IFormattable, IConvertible, IComparable<TValue>, IEquatable<TValue>
    {
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
