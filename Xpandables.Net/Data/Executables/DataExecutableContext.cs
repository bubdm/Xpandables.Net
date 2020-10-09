
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
using System.Threading.Tasks;

using Xpandables.Net.Data.Connections;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Defines the context of an executable.
    /// </summary>
    public sealed class DataExecutableContext : Disposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableContext"/> class with the argument and connection.
        /// </summary>
        /// <param name="argument">The context argument.</param>
        /// <param name="connectionContext">The component argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionContext"/> is null.</exception>
        public DataExecutableContext(DataExecutableArgument argument, DataConnectionContext connectionContext)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            ConnectionContext = connectionContext ?? throw new ArgumentNullException(nameof(connectionContext));
        }

        /// <summary>
        /// Gets the argument for the executable.
        /// </summary>
        public DataExecutableArgument Argument { get; }
        
        /// <summary>
        /// Gets the component needed by the executable.
        /// </summary>
        public DataConnectionContext ConnectionContext { get; }

        private bool isDisposed;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
                ConnectionContext.Dispose();

            isDisposed = true;
        }

        /// <summary>
        /// Asynchronously disposes the connection.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected override async ValueTask DisposeAsync(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
                await ConnectionContext.DisposeAsync().ConfigureAwait(false);

            isDisposed = true;
        }
    }
}
