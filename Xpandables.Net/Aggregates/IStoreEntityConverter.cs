
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
    /// Converter to be used with <see cref="StoreEntity"/>
    /// </summary>
    public interface IStoreEntityConverter
    {
        /// <summary>
        /// Converts the value of a specified type into a JSON string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="inputType">The type of the value to convert.</param>
        /// <returns>The JSON string representation of the value.</returns>
        string Serialize(object value, Type inputType);

        /// <summary>
        /// Parses the text representing a single JSON value into an instance of a specified type.
        /// </summary>
        /// <param name="value">The JSON text to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <returns>A returnType representation of the JSON value.</returns>
        object Deserialize(string value, Type returnType);
    }
}
