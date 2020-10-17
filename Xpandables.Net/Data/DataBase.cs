
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

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Executables;
using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with a default implementation of <see cref="IDataBase"/>.
    /// </summary>
    public sealed class DataBase : IDataBase
    {
        private readonly IDataConnectionContextProvider _dataConnectionContextProvider;
        private readonly IDataExecutableProvider _dataExecutableProvider;
        private readonly IDataConnectionOptions? _dataConnection;
        private readonly IDataExecutableOptions? _dataOptions;

        IDataExecutableProvider IDataBase.DataExecutableProvider => _dataExecutableProvider;
        IDataConnectionContextProvider IDataBase.DataConnectionContextProvider => _dataConnectionContextProvider;
        IDataConnectionOptions? IDataBase.DataConnection => _dataConnection;
        IDataExecutableOptions? IDataBase.DataOptions => _dataOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/>.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data connection.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataConnection">The default data connection to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataConnectionOptions dataConnection)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data options.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataOptions">The default data options to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataExecutableOptions dataOptions)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataOptions = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DataBase"/> with a default data options and connection.
        /// </summary>
        /// <param name="dataConnectionContextProvider">The data context provider.</param>
        /// <param name="dataExecutableProvider">The data executable provider.</param>
        /// <param name="dataConnection">The default data connection to be used.</param>
        /// <param name="dataOptions">The default data options to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnectionContextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataExecutableProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataOptions"/> is null.</exception>
        public DataBase(IDataConnectionContextProvider dataConnectionContextProvider, IDataExecutableProvider dataExecutableProvider, IDataConnectionOptions dataConnection, IDataExecutableOptions dataOptions)
        {
            _dataConnectionContextProvider = dataConnectionContextProvider ?? throw new ArgumentNullException(nameof(dataConnectionContextProvider));
            _dataExecutableProvider = dataExecutableProvider ?? throw new ArgumentNullException(nameof(dataExecutableProvider));
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _dataOptions = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
        }
    }
}
