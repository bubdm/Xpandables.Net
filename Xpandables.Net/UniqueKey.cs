
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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Xpandables.Net;

/// <summary>
/// Represents a helper class to implement a key of a specific type.
/// </summary>
/// <typeparam name="TId">The type of the key value.</typeparam>
[Serializable]
public abstract class UniqueKey<TId> : IEqualityComparer<UniqueKey<TId>>, IEquatable<UniqueKey<TId>>, IComparable<UniqueKey<TId>>, IUniqueKey<TId>
    where TId : notnull, IComparable
{
    ///<inheritdoc/>
    public TId Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="UniqueKey{TId}"/> with the specified value.
    /// </summary>
    /// <param name="value">The value for the new instance.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
    [JsonConstructor]
    protected UniqueKey(TId value) => Value = value ?? throw new ArgumentNullException(nameof(value));

    ///<inheritdoc/>
    public bool Equals(UniqueKey<TId>? x, UniqueKey<TId>? y) => x?.Equals(y) ?? false;

    ///<inheritdoc/>
    public int GetHashCode([DisallowNull] UniqueKey<TId> obj) => Value.GetHashCode();

    ///<inheritdoc/>
    public bool Equals(UniqueKey<TId>? other) => other is not null && GetType() == other.GetType() && Value.Equals(other.Value);

    ///<inheritdoc/>
    public int CompareTo(UniqueKey<TId>? other) => other is null ? -1 : Value.CompareTo(other.Value);

    ///<inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as UniqueKey<TId>);

    ///<inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// When overridden, this method should determine whether or not the key contains a valid value or not.
    /// </summary>
    /// <returns><see langword="true"/> if it's defined, otherwise <see langword="false"/>.</returns>
    public abstract bool IsEmpty();

    /// <summary>
    /// When overridden, this method should return the <see cref="string"/> representation of the key value.
    /// </summary>
    /// <returns>A <see cref="string"/> value.</returns>
    public abstract string AsString();

    ///<inheritdoc/>
    public static bool operator ==(UniqueKey<TId> left, UniqueKey<TId> right) => left?.Equals(right) ?? right is null;

    ///<inheritdoc/>
    public static bool operator !=(UniqueKey<TId> left, UniqueKey<TId> right) => !(left == right);

    ///<inheritdoc/>
    public static bool operator <(UniqueKey<TId> left, UniqueKey<TId> right) => left is null ? !(right is null) : left.CompareTo(right) < 0;

    ///<inheritdoc/>
    public static bool operator <=(UniqueKey<TId> left, UniqueKey<TId> right) => left is null || left.CompareTo(right) <= 0;

    ///<inheritdoc/>
    public static bool operator >(UniqueKey<TId> left, UniqueKey<TId> right) => !(left is null) && left.CompareTo(right) > 0;

    ///<inheritdoc/>
    public static bool operator >=(UniqueKey<TId> left, UniqueKey<TId> right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
