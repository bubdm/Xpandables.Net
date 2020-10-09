
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
using System.Runtime.CompilerServices;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Options;

[assembly:InternalsVisibleTo("Xpandables.Net.DependencyInjection, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]
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
        public DataBaseOptions UseDataConnection(IDataConnectionOptions dataConnection)
            => this.Assign(dbo => dbo.IsDataConnectionEnabled= dataConnection ?? throw new ArgumentNullException(nameof(dataConnection)));

        internal IDataOptions? IsDataOptionsEnabled { get; private set; }
        internal IDataConnectionOptions? IsDataConnectionEnabled { get; private set; }
    }
}
