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
using System.ComponentModel.DataAnnotations;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Defines the <see cref="Name"/> class that holds a <see cref="FirstName"/> and a <see cref="LastName"/> properties.
    /// </summary>
    public class Name : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Name"/> with values.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="firstName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lastName"/> is null.</exception>
        protected Name(string lastName, string firstName)
        {
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        }

        /// <summary>
        /// Creates a new instance of <see cref="Name"/> with values provided.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <returns>An instance of <see cref="Name"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="firstName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lastName"/> is null.</exception>
        public static Name Create(string firstName, string lastName) => new Name(lastName, firstName);

        /// <summary>
        /// Creates a default instance of <see cref="Name"/> with NO value.
        /// </summary>
        public static Name Default() => Create("NO FIRSTNAME", "NO LASTNAME");

        /// <summary>
        /// Provides with deconstruction for <see cref="Name"/>.
        /// </summary>
        /// <param name="firstName">The output first name.</param>
        /// <param name="lastName">The output last name.</param>
        public void Deconstruct(out string firstName, out string lastName) => (firstName, lastName) = (FirstName, LastName);

        /// <summary>
        /// Gets the last name.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        [Required, StringLength(byte.MaxValue, MinimumLength = 3)]
        public string FirstName { get; private set; }

        /// <summary>
        /// Updates the first name and return the current instance.
        /// </summary>
        /// <param name="firstName">The new first name.</param>
        public Name EditFirstName(string firstName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            return this;
        }

        /// <summary>
        /// Updates the last name and return the current instance.
        /// </summary>
        /// <param name="lastName">The new last name.</param>
        public Name EditLastName(string lastName)
        {
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{FirstName} {LastName}";

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LastName;
            yield return FirstName;
        }
    }
}
