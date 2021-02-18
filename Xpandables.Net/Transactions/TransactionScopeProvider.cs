
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
using System.Transactions;

using Xpandables.Net.Decorators;

namespace Xpandables.Net.Transactions
{
    /// <summary>
    /// Provides a default implementation of <see cref="ITransactionScopeProvider"/>.
    /// You can derive from this class to customize its behavior.
    /// </summary>
    public class TransactionScopeProvider : ITransactionScopeProvider
    {
        /// <summary>
        /// Returns an instance that contains the transaction scope to be used.
        /// The transaction scope used the <see cref="TransactionScopeAsyncFlowOption.Enabled"/> option.
        /// </summary>
        /// <param name="argument">The command/query instance to retrieve the transaction scope for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public TransactionScope? GetTransactionScope<TArgument>(TArgument argument)
            where TArgument : class, ITransactionDecorator
            => new(TransactionScopeAsyncFlowOption.Enabled);
    }
}
