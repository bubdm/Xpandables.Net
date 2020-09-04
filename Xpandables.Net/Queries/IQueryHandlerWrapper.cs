
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

namespace Xpandables.Net.Queries
{
    /// <summary>
    /// This interface allows application authors to avoid use of C# dynamics with query pattern and allows type inference.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IQueryHandlerWrapper<TResult>
    {
        /// <summary>
        /// Handles the specified query and returns the expected result type.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        /// <returns>An object that represents the result of <typeparamref name="TResult"/>.</returns>
        TResult Handle(IQuery<TResult> query);
    }
}