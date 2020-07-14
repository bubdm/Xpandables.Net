
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
    /// Enumeration of multi factor authentication. This class can be extended.
    /// </summary>
    [Description("Defines the multi factor authentication.")]
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public class MultiFactorType : EnumerationType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultiFactorType"/> class.
        /// </summary>
        /// <param name="value">The value of the person type.</param>
        /// <param name="displayName">The name of the person type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="displayName"/> is null.</exception>
        protected MultiFactorType(int value, string displayName) : base(displayName, value) { }

        /// <summary>
        /// MultiFactor authentication using SMS.
        /// </summary>
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public static MultiFactorType MFA_SMS => new MultiFactorType(1, nameof(MFA_SMS));

        /// <summary>
        /// MultiFactor authentication using a Time-based One-time Password Algorithm.
        /// </summary>
        public static MultiFactorType MFA_TOTP => new MultiFactorType(2, nameof(MFA_TOTP));
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
