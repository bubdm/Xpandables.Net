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

namespace Xpandables.Net.Expressions.Specifications
{
    /// <summary>
    /// Provides the specification factory that contains methods to create generic specifications.
    /// </summary>
    public static class SpecificationFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="Specification{TSource}"/> with <see cref="bool"/> result that return <see langword="true"/>.
        /// </summary>
        /// <typeparam name="TSource">The data type source.</typeparam>
        /// <returns>a new instance of <see cref="Specification{TSource}"/> with boolean result.</returns>
        public static Specification<TSource> Create<TSource>() where TSource : notnull => new SpecificationBuilder<TSource>(_ => true);
    }
}
