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

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// Default validation attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class ValidationRuleAttribute : ValidationAttribute { }

    /// <summary>
    /// Provides with helper methods for validation attributes.
    /// </summary>
    public static class ValidationAttributeExtensions
    {
        /// <summary>
        /// Helper uses to build a <see cref="ValidationException"/> using the specified arguments.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger the validation error.</param>
        /// <param name="memberNames"> The list of member names that have validation errors.</param>
        public static ValidationException ValidationExceptionBuilder(
            this object _, string errorMessage, object? value, params string[] memberNames)
            => new ValidationException(
                new ValidationResult(errorMessage, memberNames),
                new ValidationRuleAttribute(),
                value);

        /// <summary>
        /// Helper uses to build a <see cref="ValidationException"/> from the current attribute using the specified arguments.
        /// </summary>
        /// <param name="this">The current validation attribute.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger the validation error.</param>
        /// <param name="memberNames"> The list of member names that have validation errors.</param>
        public static ValidationException ValidationExceptionBuilder(
                  this ValidationAttribute @this, string errorMessage, object? value, params string[] memberNames)
                  => new ValidationException(
                      new ValidationResult(errorMessage, memberNames),
                      @this,
                      value);
    }
}