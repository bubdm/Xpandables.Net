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

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Transactions
{
    /// <summary>
    /// A marker interface that allows the handler class implementation to be decorated with transaction behavior according to
    /// the decorated class type :
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryTransactionBehavior{TQuery, TResult}"/>.
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandTransactionBehavior{TCommand}"/>.
    /// You must implement the <see cref="ITransactionScopeProvider"/> to provide the transaction scope.
    /// <para></para>
    /// You need to register the expected behavior to the service collections using the appropriate extension method.
    /// </summary>
    public interface IBehaviorTransaction { }
}
