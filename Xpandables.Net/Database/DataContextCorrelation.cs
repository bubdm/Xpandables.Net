
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
using System.Linq;

namespace Xpandables.Net.Database
{
    internal sealed class DataContextCorrelation : IDataContextCorrelation
    {
        private readonly IEnumerable<IDataContextFactory> _dataContextFactories;

        /// <summary>
        /// Good
        /// </summary>
        /// <param name="dataContextFactories"></param>
        public DataContextCorrelation(IEnumerable<IDataContextFactory> dataContextFactories)
        {
            _dataContextFactories = dataContextFactories ?? throw new ArgumentNullException(nameof(dataContextFactories));
        }

        /// <summary>
        /// Gets the name of the ambient data context.
        /// </summary>
        public string? CurrentDataContextName { get; private set; }

        /// <summary>
        /// Returns an instance of the ambient data context.
        /// </summary>
        /// <returns><see cref="IDataContext" /> derived class.</returns>
        public IDataContext GetDataContext()
        {
            _ = CurrentDataContextName ?? throw new ArgumentNullException(nameof(CurrentDataContextName), "The data context name has not been set.");
            if (_dataContextFactories.FirstOrDefault(provider => provider.Name == CurrentDataContextName) is { } factory)
                return factory.Factory();

            throw new InvalidOperationException($"The '{CurrentDataContextName}' has not been registered.");
        }

        /// <summary>
        /// Sets the ambient data context name.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        public void SetCurrentDataContextName<TDataContext>()
            where TDataContext : class, IDataContext => CurrentDataContextName = typeof(TDataContext).Name;

        /// <summary>
        /// Sets the ambient data context name.
        /// </summary>
        /// <param name="name">The name of the data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> is null.</exception>
        public void SetCurrentDataContextName(string name) => CurrentDataContextName = name ?? throw new ArgumentNullException(nameof(name));
    }
}
