
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

using Xpandables.Net5.Enumerations;

namespace Xpandables.Net5.Entities
{
    /// <summary>
    /// Enumeration of person type. This class can be extended.
    /// </summary>
    [Description("Describes a person type.")]
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public class PersonType : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PersonType"/> class.
        /// </summary>
        /// <param name="value">The value of the person type.</param>
        /// <param name="displayName">The name of the person type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName"/> is null.</exception>
        protected PersonType(int value, string displayName) : base(displayName, value) { }

        /// <summary>
        /// Determines whether the current instance is a <see cref="Natural"/>.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsNatural() => this == Natural;

        /// <summary>
        /// Determines whether the current instance is a <see cref="Legal"/>.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsLegal() => this == Legal;

        /// <summary>
        /// The natural person type.
        /// </summary>
        public static PersonType Natural => new PersonType(1, nameof(Natural));

        /// <summary>
        /// The legal person type.
        /// </summary>
        public static PersonType Legal => new PersonType(2, nameof(Legal));
    }
}
