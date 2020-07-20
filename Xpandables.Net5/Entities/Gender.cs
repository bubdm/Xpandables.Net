
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
using System.ComponentModel.DataAnnotations;

using Xpandables.Net5.Enumerations;

namespace Xpandables.Net5.Entities
{
    /// <summary>
    /// Enumeration of gender.  This class can be extended.
    /// </summary>
    [Description("Describes a person gender.")]
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public class Gender : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Gender"/> class.
        /// </summary>
        /// <param name="value">The value of the gender.</param>
        /// <param name="displayName">The name of the gender.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName"/> is null.</exception>
        protected Gender(int value, string displayName) : base(displayName, value) { }

        /// <summary>
        /// Determines whether the current instance is a <see cref="Man"/>.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsMan() => this == Man;

        /// <summary>
        /// Determines whether the current instance is a <see cref="Woman"/>.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsWoman() => this == Woman;

        /// <summary>
        /// Determines whether the current instance is a <see cref="Undefined"/>.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsUndefinied() => this == Undefined;

        /// <summary>
        /// The undefined gender.
        /// </summary>
        [Display(Name = nameof(Undefined))]
        public static Gender Undefined => new Gender(0, nameof(Undefined));

        /// <summary>
        /// The Man gender.
        /// </summary>
        [Display(Name = nameof(Man))]
        public static Gender Man => new Gender(1, nameof(Man));

        /// <summary>
        /// The Woman gender.
        /// </summary>
        [Display(Name = nameof(Woman))]
        public static Gender Woman => new Gender(2, nameof(Woman));
    }
}
