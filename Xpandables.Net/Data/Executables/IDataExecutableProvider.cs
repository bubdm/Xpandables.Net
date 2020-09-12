
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

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Allows application author to provide with an executable of a specific type.
    /// </summary>
    public interface IDataExecutableProvider
    {
        /// <summary>
        /// Returns the executable that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable.</typeparam>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        public TDataExecutable? GetDataExecutable<TResult, TDataExecutable>()
            where TDataExecutable : DataExecutable<TResult>;

        /// <summary>
        /// Returns the executable mapper that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapper">The type of the executable.</typeparam>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        public TDataExecutableMapper? GetDataExecutableMapper<TResult, TDataExecutableMapper>()
            where TDataExecutableMapper : DataExecutableMapper<TResult>
            where TResult : class, new();
    }

    /// <summary>
    /// Provides with a method to retrieve data executable instance using the <see cref="IServiceProvider"/>.
    /// </summary>
    public sealed class DataExecutableProvider : IDataExecutableProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableProvider"/> with the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public DataExecutableProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <summary>
        /// Returns the executable that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutable">The type of the executable.</typeparam>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        public TDataExecutable? GetDataExecutable<TResult, TDataExecutable>()
            where TDataExecutable : DataExecutable<TResult>
            => _serviceProvider.GetService(typeof(TDataExecutable)) as TDataExecutable;

        /// <summary>
        /// Returns the executable mapper that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TDataExecutableMapper">The type of the executable.</typeparam>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        public TDataExecutableMapper? GetDataExecutableMapper<TResult, TDataExecutableMapper>()
            where TDataExecutableMapper : DataExecutableMapper<TResult>
            where TResult : class, new()
            => _serviceProvider.GetService(typeof(TDataExecutableMapper)) as TDataExecutableMapper;
    }
}
