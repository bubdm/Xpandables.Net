
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
        /// Returns the executable that matches the type if found.
        /// </summary>
        /// <param name="executableType">The executable type to search for.</param>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executableType"/> is null.</exception>
        object? GetExecutable(Type executableType);

        /// <summary>
        /// Returns the executable that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TExecutable">The type of the executable.</typeparam>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        public TExecutable? GetExecutable<TExecutable>()
            where TExecutable : class, IDataExecutable
            => GetExecutable(typeof(TExecutable)) as TExecutable;

        /// <summary>
        /// Returns the executable mapped that matches the specific type if found.
        /// </summary>
        /// <typeparam name="TExecutableMapped">The type of the executable mapped.</typeparam>
        /// <returns>An instance of the executable mapped type if found, otherwise null.</returns>
        public TExecutableMapped? GetExecutableMapped<TExecutableMapped>()
            where TExecutableMapped : class, IDataExecutableMapper
            => GetExecutable(typeof(TExecutableMapped)) as TExecutableMapped;
    }

    /// <summary>
    /// Default implementation of <see cref="IDataExecutableProvider"/> that uses service provider to retrieve executables.
    /// </summary>
    public sealed class DataExecutableProvider : IDataExecutableProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableProvider"/> with the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public DataExecutableProvider(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <summary>
        /// Returns the executable that matches the type if found.
        /// </summary>
        /// <param name="executableType">The executable type to search for.</param>
        /// <returns>An instance of the executable type if found, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executableType"/> is null.</exception>
        public object? GetExecutable(Type executableType) => _serviceProvider.GetService(executableType);

    }
}
