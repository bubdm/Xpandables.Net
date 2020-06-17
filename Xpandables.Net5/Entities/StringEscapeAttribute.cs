
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
using System.ComponentModel.DataAnnotations;

namespace System
{
    /// <summary>
    /// Escapes special characters from the decorated property. The type property must be a string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class StringEscapeAttribute : ValidationAttribute
    {
        /// <summary>Validates the specified value with respect to the current validation attribute.</summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        /// <exception cref="InvalidOperationException">The current attribute is malformed.</exception>
        /// <exception cref="NotImplementedException">
        ///   <see cref="ValidationAttribute.IsValid(object,ValidationContext)" /> has not been implemented by a derived class.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _ = validationContext ?? throw new ArgumentNullException(nameof(validationContext));

            return value switch
            {
                null => ValidationResult.Success,
                string source => StringIsValid(source),
                _ => ValidationResult.Success
            };

            ValidationResult StringIsValid(string source)
            {
                validationContext
                    .ObjectType
                    .GetProperty(validationContext.MemberName)
                    ?.SetValue(validationContext.ObjectInstance, source.StringEscape());

                return ValidationResult.Success;
            }
        }
    }
}
