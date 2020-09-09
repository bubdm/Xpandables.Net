
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

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// Specifies that a data field value is optionally required settings the <see cref="IsOptional"/> to <see langword="true"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class StringLengthOptionalAttribute : StringLengthAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLengthOptionalAttribute"/> class by using a specified maximum length.
        /// </summary>
        /// <param name="maximumLength"> The maximum length of a string.</param>
        public StringLengthOptionalAttribute(int maximumLength) : base(maximumLength) { }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the System.ComponentModel.DataAnnotations.ValidationResult class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null && IsOptional)
                return ValidationResult.Success;

#nullable disable
            return base.IsValid(value, validationContext);
#nullable enable
        }

        /// <summary>
        /// Determines whether or not the decorated property/field/parameter can be null.
        /// </summary>
        public bool IsOptional { get; set; }
    }
}
