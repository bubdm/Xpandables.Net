
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Implementation of <see cref="IDataContextFactoryCollection"/>.
    /// </summary>
    public sealed class DataContextFactoryCollection : IDataContextFactoryCollection
    {
        private readonly IDictionary<string, IDataContextFactory> _dataContextFactories;

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextFactoryCollection"/> class with all the data context factories.
        /// </summary>
        /// <param name="dataContextFactories">The collection of data context factories.</param>
        public DataContextFactoryCollection(IEnumerable<IDataContextFactory> dataContextFactories)
        {
            _dataContextFactories = dataContextFactories?.ToDictionary(d => d.Name, d => d) ?? new Dictionary<string, IDataContextFactory>();
        }

        /// <summary>
        /// Returns the data context matching the specified name. If not found returns null.
        /// </summary>
        /// <param name="name">The data context name to search for.</param>
        /// <returns>The requested data context or null if not present.</returns>
        public IDataContext? this[string name] =>
            _dataContextFactories.TryGetValue(name, out var factory) ? factory.Factory() : default;

        /// <summary>
        /// Gets the name of the ambient data context.
        /// </summary>
        public string? CurrentDataContextName { get; private set; }

        /// <summary>
        /// Returns an instance of the ambient data context matching the <see cref="CurrentDataContextName" />.
        /// </summary>
        /// <returns><see cref="IDataContext" /> derived class.</returns>
        /// <exception cref="InvalidOperationException">The data context matching the current has not been registered.</exception>
        /// <exception cref="ArgumentNullException">The <see cref="CurrentDataContextName" /> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ", Justification = "<Pending>")]
        public IDataContext GetDataContext()
        {
            _ = CurrentDataContextName ?? throw new ArgumentNullException(nameof(CurrentDataContextName), "The data context name has not been set.");
            if (_dataContextFactories.TryGetValue(CurrentDataContextName, out var factory))
                return factory.Factory();

            throw new InvalidOperationException(
                $"The '{CurrentDataContextName}' factory has not been registered. Use services.AddXDataContextFactory<{CurrentDataContextName}>() to register the factory.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, IDataContextFactory>> GetEnumerator()
        {
            foreach (var pair in _dataContextFactories)
                yield return pair;
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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
