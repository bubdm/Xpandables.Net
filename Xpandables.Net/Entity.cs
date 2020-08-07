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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("Xpandables.Net.EntityFramework, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]

namespace Xpandables.Net
{
    /// <summary>
    /// The domain object base implementation that provides an identifier and a key generator for derived class.
    /// This is an <see langword="abstract"/>class.
    /// <para>You have to decorate the <see cref="Id"/> property with key attributes or use another one.</para>
    /// </summary>
    [DebuggerDisplay("Id = {Id}")]
    public abstract class Entity
    {
        /// <summary>
        /// Initializes the Id key.
        /// </summary>
        protected Entity() => Id = DoKeyGenerator();

        private string DoKeyGenerator() => KeyGenerator();

        /// <summary>
        /// Gets the domain object identity.
        /// The value comes from <see cref="KeyGenerator"/>.
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Determines whether or not the underlying instance is new.
        /// The default implementation just compare the <see cref="Id"/> value to its default one.
        /// You must override this property in order to match your request.
        /// </summary>
        public bool IsNew() => string.IsNullOrWhiteSpace(Id);

        /// <summary>
        /// Determines whether or not the underlying instance is deleted.
        /// </summary>
        public bool IsDeleted { get; protected set; }

        /// <summary>
        /// Determines whether or not the underlying instance is active.
        /// </summary>
        public bool IsActive { get; protected set; } = true;

        /// <summary>
        /// Gets the creation date of the underlying instance.
        /// </summary>
        public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the last update date of the underlying instance if exist.
        /// </summary>
        public DateTime? UpdatedOn { get; protected set; }

        /// <summary>
        /// Gets the deleted date of the underlying instance if exist.
        /// </summary>
        public DateTime? DeletedOn { get; protected set; }

        /// <summary>
        /// Deactivates the underlying instance.
        /// </summary>
        public void Deactivate() => IsActive = false;

        /// <summary>
        /// Deletes the underlying instance.
        /// </summary>
        public void Delete() => IsDeleted = true;

        /// <summary>
        /// Activates the underlying instance.
        /// </summary>
        public void Activate() => IsActive = true;

        /// <summary>
        /// Sets the creation date time for underlying instance.
        /// </summary>
        /// <param name="dateTime">the creation date.</param>
        internal void SetCreationDate(DateTime dateTime) => CreatedOn = dateTime;

        /// <summary>
        /// Sets the last update date time for underlying instance.
        /// </summary>
        /// <param name="dateTime">the last updated date.</param>
        internal void SetUpdateDate(DateTime dateTime) => UpdatedOn = dateTime;

        /// <summary>
        /// Sets the deletion date time for underlying instance.
        /// </summary>
        /// <param name="dateTime">the deletion date.</param>
        internal void SetDeleteDate(DateTime dateTime) => DeletedOn = dateTime;

        /// <summary>
        /// Returns the unique signature of string type for an instance.
        /// This signature value will be used as identifier for the underlying instance.
        /// <para>When overridden in the derived class, it will set or get the concrete identity for the domain object.</para>
        /// </summary>
        /// <returns>A string value as identifier.</returns>
        protected virtual string KeyGenerator()
        {
            using var rnd = RandomNumberGenerator.Create();
            var salt = new byte[32];
            var guid = Guid.NewGuid().ToString();
            rnd.GetBytes(salt);

            return $"{guid}{BitConverter.ToString(salt)}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is Entity other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id) && Id == other.Id;
        }

        /// <summary>
        /// Applies equal operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Entity a, Entity b)
            => a is null && b is null || !(a is null) && !(b is null) && a.Equals(b);

        /// <summary>
        /// Applies non equal operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Entity a, Entity b) => !(a == b);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current entity.</returns>
        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}