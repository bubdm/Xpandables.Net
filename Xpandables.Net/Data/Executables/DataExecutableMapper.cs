﻿
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
using System.Collections.Generic;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Provides with a base definition of an executable process for <see cref="DataBase"/> that maps the result to the target type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result to map to.</typeparam>
    public abstract class DataExecutableMapper<TResult>
        where TResult : class, new()
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns the result mapped to the specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>An asynchronous enumeration of <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public abstract IAsyncEnumerable<TResult> ExecuteMappedAsync(DataExecutableContext context);
    }
}
