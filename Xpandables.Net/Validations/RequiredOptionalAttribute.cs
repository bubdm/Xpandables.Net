
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

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// Specifies that a data field value is required.
    /// When the <see cref="IsOptional"/> is <see langword="true"/>, the data field is only checked if there is a value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RequiredOptionalAttribute : RequiredAttribute
    {
        /// <summary>
        /// Checks that the value of the required data field is not empty if optional is <see langword="false"/>.
        /// </summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns> true if validation is successful; otherwise, false.</returns>
        /// <exception cref="ValidationException">The data field value was null.</exception>
        public override bool IsValid(object? value)
        {
            if (IsOptional && value is null)
                return true;

            return base.IsValid(value);
        }

        /// <summary>
        /// Gets or sets the value whether or not the decorated property/field/parameter can be null.
        /// If <see langword="true"/>, the data field will only be checked when there is a value. The default behavior is <see langword="false"/>.
        /// </summary>
        public bool IsOptional { get; set; }
    }
}
