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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// This interface is used as a marker for queries when using the synchronous query pattern that contains a specific-type result.
    /// Class implementation is used with the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> where
    /// "TQuery" is a class that implements the <see cref="IAsyncQuery{TResult}"/> interface. This can also be enhanced with some useful decorators.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the query.</typeparam>
    public interface IAsyncQuery<out TResult> { }
}