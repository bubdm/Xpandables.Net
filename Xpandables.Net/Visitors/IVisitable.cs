﻿/************************************************************************************************************
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

namespace Xpandables.Net.Visitors;

/// <summary>
/// Defines an Accept operation that takes a visitor as an argument.
/// Visitor design pattern allows you to add new behaviors to an existing object without changing the object structure.
/// The implementation must be thread-safe when working in a multi-threaded environment.
/// </summary>
/// <typeparam name="TVisitable">The type of the object to act on.</typeparam>
public interface IVisitable<out TVisitable>
    where TVisitable : class, IVisitable<TVisitable>
{
    /// <summary>
    /// Defines the Accept operation.
    /// When overridden in derived class, this method will accept the specified visitor.
    /// The default behavior just call the visit method of the visitor on the current instance.
    /// </summary>
    /// <param name="visitor">The visitor to be applied on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
    public virtual async Task AcceptAsync(IVisitor<TVisitable> visitor, CancellationToken cancellationToken = default)
    {
        _ = visitor ?? throw new ArgumentNullException(nameof(visitor));
        await visitor.VisitAsync((TVisitable)this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Defines the Accept operation with <see cref="ICompositeVisitor{TElement}"/>.
    /// </summary>
    /// <param name="visitor">The composite visitor to be applied on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is null.</exception>
    public sealed async Task AcceptAsync(ICompositeVisitor<TVisitable> visitor, CancellationToken cancellationToken = default)
    {
        _ = visitor ?? throw new ArgumentNullException(nameof(visitor));
        await visitor.VisitAsync((TVisitable)this, cancellationToken).ConfigureAwait(false);
    }
}
