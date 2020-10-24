
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static Xpandables.Net.Validations.ValidationAttributeExtensions;

namespace Xpandables.Net.Api.Models
{
    public sealed class EmailAddress : ValueObject
    {
        private const string EmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public EmailAddress(string value) => Value = AssertIsNotNullAndValid(value);

        public string Value { get; }

        private static string AssertIsNotNullAndValid(string value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            try
            {
                return Regex.IsMatch(value, EmailRegex)
                    ? value
                    : throw CreateValidationException($"Invalid email address", value, new[] { "Email" });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is RegexMatchTimeoutException)
            {
                throw CreateValidationException($"Invalid email address", value, new[] { "Email" });
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(EmailAddress emailAddress) => emailAddress.Value;
        public static explicit operator EmailAddress(string value) => new EmailAddress(value);
    }
}
