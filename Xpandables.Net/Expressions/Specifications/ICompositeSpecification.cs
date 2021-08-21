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
/// Defines one method which returns boolean to assert that a collection of specifications is satisfied or not.
/// method used to check whether or not the specification is satisfied by the <typeparamref name="TSource"/> object.
/// </summary>
/// <typeparam name="TSource">Type of the argument to be validated.</typeparam>
public interface ICompositeSpecification<TSource> : ISpecification<TSource>
    where TSource : notnull
{ }
