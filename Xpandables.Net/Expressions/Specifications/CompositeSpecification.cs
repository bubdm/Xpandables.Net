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

namespace Xpandables.Net.Expressions.Specifications;

/// <summary>
/// The composite specification class used to wrap all specifications for a specific type.
/// </summary>
/// <typeparam name="TSource">The type of the object to check for.</typeparam>
[Serializable]
public class CompositeSpecification<TSource> : Specification<TSource>, ICompositeSpecification<TSource>
    where TSource : notnull
{
    private readonly IEnumerable<ISpecification<TSource>> _specificationInstances;

    /// <summary>
    /// Initializes the composite specification with all specification instances for the argument.
    /// </summary>
    /// <param name="specificationInstances">The collection of specifications to act with.</param>
    public CompositeSpecification(IEnumerable<ISpecification<TSource>> specificationInstances)
        => _specificationInstances = specificationInstances;

    /// <summary>
    /// Returns a value that determines whether or not the specification is satisfied by the source object.
    /// </summary>
    /// <param name="source">The target source to check specification on.</param>
    /// <returns><see langword="true" /> if the specification is satisfied, otherwise <see langword="false" /></returns>
    /// <exception cref="ArgumentNullException">The <paramref name="source" /> is null.</exception>
    public override bool IsSatisfiedBy(TSource source) => _specificationInstances.All(spec => spec.IsSatisfiedBy(source));
}
