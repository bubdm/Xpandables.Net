
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

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// This class allows the application author to add seed support to data context.
    /// The target command should implement the <see cref="IInitializerDecorator"/> interface in order to activate the behavior.
    /// The class decorates the target <see cref="IDataContextProvider"/> with an implementation of
    /// <see cref="IDataContextInitializer"/> that will be called before the data context is returned.
    /// </summary>
    public sealed class DataContextInitializerDecorator : IDataContextProvider
    {
        private readonly IDataContextProvider _decoratee;
        private readonly IDataContextInitializer _initializer;

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextInitializerDecorator"/>.
        /// </summary>
        /// <param name="datacontextProvider">The decorated data context provider.</param>
        /// <param name="dataContextInitializer">The data context seeder.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="datacontextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContextInitializer"/> is null.</exception>
        public DataContextInitializerDecorator(IDataContextProvider datacontextProvider, IDataContextInitializer dataContextInitializer)
        {
            _decoratee = datacontextProvider ?? throw new ArgumentNullException(nameof(datacontextProvider));
            _initializer = dataContextInitializer ?? throw new ArgumentNullException(nameof(dataContextInitializer));
        }

        async Task<IDataContext> IDataContextProvider.GetDataContextAsync(System.Threading.CancellationToken cancellationToken)
        {
            var context = await _decoratee.GetDataContextAsync(cancellationToken).ConfigureAwait(false);
            await _initializer.InitializeAsync(context, cancellationToken).ConfigureAwait(false);

            return context;
        }

        IDataContext IDataContextProvider.GetDataContext()
        {
            var context = _decoratee.GetDataContext();
            _initializer.Initialize(context);

            return context;
        }
    }
}