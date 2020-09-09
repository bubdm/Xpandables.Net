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

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// A marker interface that allows the command/query class to add correlation decorator context event after control flow.
    /// In the class handling the query or command, you should reference
    /// the <see cref="ICorrelationContext"/> and set the <see cref="ICorrelationContext.PostEvent"/> and/or
    /// <see cref="ICorrelationContext.RollbackEvent"/>.
    /// <para></para>
    /// Note that <see cref="ICorrelationContext.PostEvent"/> will be raised at the end of the control only if there is no exception,
    /// giving you access to all data still alive on the control flow and the <see cref="ICorrelationContext.RollbackEvent"/>
    /// will only be raised when exception. The exception in that case in accessible through the
    /// <see cref="ICorrelationContext.RollbackEvent"/>.
    /// <para></para>
    /// You need to register the expected behavior using the appropriate extension method.
    /// <para></para>
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryCorrelationDecorator{TQuery, TResult}"/>.
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandCorrelationDecorator{TCommand}"/>.
    /// <see cref="IAsyncQuery{TResult}"/> class implementation will be decorated with <see cref="AsyncQueryCorrelationDecorator{TQuery, TResult}"/>.
    /// <see cref="IAsyncCommand"/> class implementation will be decorated with <see cref="AsyncCommandCorrelationDecorator{TCommand}"/>.
    /// </summary>
    public interface ICorrelationDecorator { }
}
