
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

namespace Xpandables.Net
{
    /// <summary>
    /// Represents a property to implement a key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IUniqueKey<TKey>
        where TKey : notnull, IComparable
    {
        /// <summary>
        /// Gets the value of the key.
        /// </summary>
        TKey Value { get; }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the key value.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        string AsString();

        /// <summary>
        /// Returns a value that determine whether or not the key is defined or empty.
        /// </summary>
        /// <returns><see langword="true"/> if it's defined, otherwise <see langword="false"/>.</returns>
        bool IsEmpty();
    }
}