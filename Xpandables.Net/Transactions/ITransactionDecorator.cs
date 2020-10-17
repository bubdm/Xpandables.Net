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
    /// A marker interface that allows the command handler class implementation to be decorated with transaction behavior according to
    /// the decorated class type :
    /// <see cref="IAsyncCommand"/> class implementation will be decorated with <see cref="AsyncCommandTransactionDecorator{TCommand}"/>.
    /// You must implement the <see cref="ITransactionScopeProvider"/> to provide the transaction scope.
    /// </summary>
    public interface ITransactionDecorator { }
}
