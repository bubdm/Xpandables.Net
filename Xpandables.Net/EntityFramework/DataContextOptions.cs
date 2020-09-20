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
[assembly: InternalsVisibleTo("Xpandables.Net.DependencyInjection, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// Defines options to configure data context.
    /// </summary>
    public sealed class DataContextOptions
    {
        /// <summary>
        /// Enables seeder for the data context that is decorated with <see cref="ISeedDecorator"/>.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</typeparam>
        /// <typeparam name="TDataContext">The type of data context.</typeparam>
        public DataContextOptions UseSeederDecorator<TDataContextSeeder, TDataContext>()
            where TDataContextSeeder : class, IDataContextSeeder<TDataContext>
            where TDataContext : class, IDataContext, ISeedDecorator
            => this.Assign(cq => cq.IsSeederEnabled = (typeof(TDataContextSeeder), typeof(TDataContext)));

        /// <summary>
        /// Enables the delegate that get called on persistence exception.
        /// If you want the exception to be re-thrown, the delegate should return an exception, otherwise null.
        /// </summary>
        /// <param name="persistenceExceptionHandler">The persistence exception handler delegate instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="persistenceExceptionHandler"/> is null.</exception>
        public DataContextOptions UsePersistenceExceptionHandler(Func<Exception, Exception?> persistenceExceptionHandler)
        {
            _ = persistenceExceptionHandler ?? throw new ArgumentNullException(nameof(persistenceExceptionHandler));
            IsPersistenceExceptionHandlerEnabled = persistenceExceptionHandler;

            return this;
        }

        internal (Type DataContextSeederType, Type DataContextType)? IsSeederEnabled { get; private set; }

        internal Func<Exception, Exception?>? IsPersistenceExceptionHandlerEnabled { get; private set; }
    }
}
