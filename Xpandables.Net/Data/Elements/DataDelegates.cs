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

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Defines a delegate to be used to build entity identity.
    /// </summary>
    /// <param name="entity">The target entity to act on.</param>
    /// <returns>A string that uniquely identifies the specified entity.</returns>
    public delegate string DataIdentityBuilder(DataEntity entity);

    /// <summary>
    /// Defines a delegate to be used for converting data row value to a specific type.
    /// </summary>
    /// <param name="property">The data property descriptor.</param>
    /// <param name="rowValue">The data row value.</param>
    /// <returns>A converted value from the data row value.</returns>
    public delegate object? DataPropertyConverter(DataProperty property, object? rowValue);
}
