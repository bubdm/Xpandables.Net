
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
    /// The target command should implement the <see cref="IBehaviorSeed"/> interface in order to activate the behavior.
    /// The class decorates the target <see cref="IDataContextProvider"/> with an implementation of
    /// <see cref="IDataContextSeeder{TDataContext}"/> that will be called before the data context is returned.
    /// </summary>
    public sealed class DataContextSeederBehavior<TDataContext> : IDataContextProvider
        where TDataContext : IDataContext, IBehaviorSeed
    {
        private readonly IDataContextProvider _decoratee;
        private readonly IDataContextSeeder<TDataContext> _seeder;

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextSeederBehavior{TDataContext}"/>.
        /// </summary>
        /// <param name="datacontextProvider">The decorated data context provider.</param>
        /// <param name="dataContextSeeder">The data context seeder.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="datacontextProvider"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContextSeeder"/> is null.</exception>
        public DataContextSeederBehavior(IDataContextProvider datacontextProvider, IDataContextSeeder<TDataContext> dataContextSeeder)
        {
            _decoratee = datacontextProvider ?? throw new ArgumentNullException(nameof(datacontextProvider));
            _seeder = dataContextSeeder ?? throw new ArgumentNullException(nameof(dataContextSeeder));
        }

        async Task<IDataContext> IDataContextProvider.GetDataContextAsync(System.Threading.CancellationToken cancellationToken)
        {
            var context = await _decoratee.GetDataContextAsync(cancellationToken).ConfigureAwait(false);
            await _seeder.SeedAsync((TDataContext)context, cancellationToken).ConfigureAwait(false);

            return context;
        }
    }
}