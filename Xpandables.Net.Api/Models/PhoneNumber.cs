
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
    public sealed class PhoneNumber : ValueObject
    {
        public const string PhoneRegex = @"^\+(?:[0-9]●?){6,14}[0-9]$";
        public PhoneNumber(string value)
            => (Value) = AssertIsValidNumberForCountry(value);

        public string Value { get; }

        public bool IsEqualTo(PhoneNumber other) => Equals(other);

        private static string AssertIsValidNumberForCountry(string value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            try
            {
                return Regex.IsMatch(value, PhoneRegex)
                    ? value
                    : throw CreateValidationException($"Invalid phone number.", value, new[] { "Phone" });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is RegexMatchTimeoutException)
            {
                throw CreateValidationException($"Invalid phone number.", value, new[] { "Phone" });
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phone) => phone.ToString();
    }
}
