
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
using System.ComponentModel.DataAnnotations;

namespace Xpandables.Net
{
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1734 // XML comment has a paramref tag, but there is no parameter by that name
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
    /// <summary>
    /// Contains properties for a token. This record can be extended using covariant return type.
    /// <para>Returns a new instance of <see cref="Xpandables.Net.ValueToken"/> with its properties.</para>
    /// </summary>
    /// <param name="Value">The value of the token string.</param>
    /// <param name="Type">The type of the token.</param>
    /// <param name="Expiry">The token expiry date.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="Value"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Type"/> is null.</exception>
    public record ValueToken([Required, DataType(DataType.Text)] string Value, [Required, DataType(DataType.Text)] string Type, [Required, DataType(DataType.DateTime)] DateTime Expiry);
}
