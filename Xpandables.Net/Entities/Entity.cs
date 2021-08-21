
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
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Xpandables.Net.Entities;

/// <summary>
/// The domain object base implementation that provides an identifier and a key generator for derived class.
/// This is an <see langword="abstract"/>class.
/// </summary>
[DebuggerDisplay("Id = {" + nameof(Id) + "}")]
[Serializable]
public abstract class Entity : IEntity
{
    /// <summary>
    /// Initializes a new instance of <see cref="Entity"/>.
    /// </summary>
    public Entity() { }

    ///<inheritdoc/>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; } = Guid.Empty;

    ///<inheritdoc/>
    public long Index { get; }

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
    public virtual bool IsNew => Id == Guid.Empty;

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
    public virtual void Created()
    {
        CreatedOn = DateTime.UtcNow;
    }

    ///<inheritdoc/>
    public void Updated() => UpdatedOn = DateTime.UtcNow;

    ///<inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    ///<inheritdoc/>
    public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode(StringComparison.OrdinalIgnoreCase);
}
