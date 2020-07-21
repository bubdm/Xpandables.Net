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

using Xpandables.Net.Helpers;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// Defines options to configure data context.
    /// </summary>
    public sealed class DataContextOptions
    {
        /// <summary>
        /// Enables seeder for the data context that is decorated with <see cref="IBehaviorSeed"/>.
        /// </summary>
        /// <typeparam name="TDataContextSeeder">The type that implements <see cref="IDataContextSeeder{TDataContext}"/>.</typeparam>
        /// <typeparam name="TDataContext">The type of data context.</typeparam>
        public DataContextOptions UseSeederBehavior<TDataContextSeeder, TDataContext>()
            where TDataContextSeeder : class, IDataContextSeeder<TDataContext>
            where TDataContext : class, IDataContext, IBehaviorSeed
            => this.With(cq => cq.IsSeederEnabled = (typeof(TDataContextSeeder), typeof(TDataContext)));

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
