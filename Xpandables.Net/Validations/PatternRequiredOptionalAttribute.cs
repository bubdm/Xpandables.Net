﻿
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
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Xpandables.Net.Strings;

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// Specifies that the data field value is required according to a regular expression by overriding the <see cref="GetRegexPattern"/> method.
    /// When the <see cref="IsOptional"/> is <see langword="true"/>, the data field is only checked if there is a value.
    /// This is an <see langword="abstract"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PatternRequiredOptionalAttribute : RequiredAttribute
    {
        /// <summary>
        /// Validates the specified value with respect to the current pattern validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult"/> class.</returns>
        /// <exception cref="InvalidOperationException">The current attribute is malformed.</exception>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            _ = validationContext ?? throw new ArgumentNullException(nameof(validationContext));

            var isValid = value is null && IsOptional;
            var memberName = validationContext.MemberName switch { string => new[] { validationContext.MemberName }, _ => default };

            if (isValid)
                return ValidationResult.Success!;

            var isRequiredValid = base.IsValid(value, validationContext);
            if (isRequiredValid != ValidationResult.Success)
                return isRequiredValid;

            if (value is not string stringValue)
                return new ValidationResult(ErrorMessageString.StringFormat(validationContext.DisplayName, "string type expected !"), memberName);

            try
            {
                isValid = Regex.IsMatch(stringValue, GetRegexPattern());
            }
            catch (Exception exception) when (exception is ArgumentException
                                            || exception is RegexMatchTimeoutException)
            {
                isValid = false;
            }

            return isValid
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessageString.StringFormat(validationContext.DisplayName), memberName);
        }

        /// <summary>
        /// Gets or sets the value whether or not the decorated property/field/parameter can be null.
        /// If <see langword="true"/>, the data field will only be checked when there is a value. The default behavior is <see langword="false"/>.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Returns the Regex pattern the value must match.
        /// </summary>
        [return: NotNull]
        protected abstract string GetRegexPattern();
    }
}