
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
    public abstract class Entity : OperationResults, IEntity
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

        ///<inheritdoc/>
        public bool IsActive { get; protected set; } = true;

        ///<inheritdoc/>
        public bool IsDeleted { get; protected set; }

        ///<inheritdoc/>
        public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;

        ///<inheritdoc/>
        public DateTime? UpdatedOn { get; protected set; }

        ///<inheritdoc/>
        public DateTime? DeletedOn { get; protected set; }

        ///<inheritdoc/>
        public virtual void Deactivated()
        {
            IsActive = false;
            UpdatedOn = DateTime.UtcNow;
        }

        ///<inheritdoc/>
        public virtual void Activated()
        {
            UpdatedOn = DateTime.UtcNow;
            IsActive = true;
        }

        ///<inheritdoc/>
        public virtual void Deleted()
        {
            IsDeleted = true;
            DeletedOn = DateTime.UtcNow;
        }

        ///<inheritdoc/>
        public virtual void Created() => CreatedOn = DateTime.UtcNow;

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
