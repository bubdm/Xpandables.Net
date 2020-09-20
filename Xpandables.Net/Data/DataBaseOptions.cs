
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
using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Defines options to configure <see cref="DataBase"/>.
    /// </summary>
    public sealed class DataBaseOptions
    {
        /// <summary>
        /// Uses the specified data options as the default for the target instance of <see cref="IDataBase"/>.
        /// </summary>
        /// <param name="dataOptions">The data options to be used.</param>
        /// <exception cref="ArgumentException">The <paramref name="dataOptions"/> is null.</exception>
        public DataBaseOptions UseDataOptions(IDataOptions dataOptions)
            => this.Assign(dbo => dbo.IsDataOptionsEnabled = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions)));

        /// <summary>
        /// Uses the specified data connection as the default for the target instance of <see cref="IDataBase"/>.
        /// </summary>
        /// <param name="dataConnection">The data connection to be used.</param>
        /// <exception cref="ArgumentException">The <paramref name="dataConnection"/> is null.</exception>
        public DataBaseOptions UseDataConnection(IDataConnection dataConnection)
            => this.Assign(dbo => dbo.IsDataConnectionEnabled= dataConnection ?? throw new ArgumentNullException(nameof(dataConnection)));

        internal IDataOptions? IsDataOptionsEnabled { get; private set; }
        internal IDataConnection? IsDataConnectionEnabled { get; private set; }
    }
}
