
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

namespace Xpandables.Net5.EntityFramework
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IDataContextProvider"/>
    /// interface without dedicated class.
    /// </summary>
    public sealed class DataContextProviderBuilder : IDataContextProvider
    {
        private readonly Func<IDataContext> _provider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextProviderBuilder"/> class with the delegate to be used
        /// as <see cref="IDataContextProvider"/> implementation.
        /// </summary>
        /// <param name="provider">The delegate to be used when the method will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IDataContextProvider"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public DataContextProviderBuilder(Func<IDataContext> provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Returns an instance that contains the ambient data context according to the environment.
        /// </summary>
        /// <returns>An instance of context that implements <see cref="IDataContext" />.</returns>
        public IDataContext GetDataContext() => _provider();
    }
}
