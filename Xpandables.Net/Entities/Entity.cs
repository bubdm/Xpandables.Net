
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
using System.Diagnostics;
using System.Security.Cryptography;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// The domain object base implementation that provides an identifier and a key generator for derived class.
    /// This is an <see langword="abstract"/>class.
    /// </summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}")]
    [Serializable]
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Initializes the Id key with <see cref="KeyGenerator"/>.
        /// </summary>
        public Entity() => Id = KeyGenerator();

        /// <summary>
        /// Gets the domain object identity.
        /// The value comes from <see cref="KeyGenerator"/>.
        /// </summary>
        [Key]
        public string Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether or not the underlying instance is marked as active.
        /// </summary>
        public bool IsActive { get; protected set; } = true;

        /// <summary>
        /// Gets a value indicating whether or not the underlying instance is marked as deleted.
        /// </summary>
        public bool IsDeleted { get; protected set; }

        /// <summary>
        /// Gets the creation date of the underlying instance. This property is automatically set by the context.
        /// </summary>
        public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the last update date of the underlying instance if exist. This property is automatically set by the context.
        /// </summary>
        public DateTime? UpdatedOn { get; protected set; }

        /// <summary>
        /// Gets the deleted date of the underlying instance if exist. This property is automatically set by the context.
        /// </summary>
        public DateTime? DeletedOn { get; protected set; }

        /// <summary>
        /// Marks the underlying instance as deactivated.
        /// </summary>
        public virtual void Deactivated() => IsActive = false;

        /// <summary>
        /// Activates the underlying instance.
        /// </summary>
        public virtual void Activated() => IsActive = true;

        /// <summary>
        /// Marks the underlying instance as deleted.
        /// </summary>
        public virtual void Deleted()
        {
            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets the creation date time for underlying instance.
        /// </summary>
        public virtual void Created() => CreatedOn = DateTime.UtcNow;

        /// <summary>
        /// Sets the last update date time for underlying instance.
        /// </summary>
        public void Updated() => UpdatedOn = DateTime.UtcNow;

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
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id) && Id == other.Id;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current entity.</returns>
        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
