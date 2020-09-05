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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// A marker interface that allows the class implementation of (<see cref="IAsyncQueryHandler{TQuery, TResult}"/> or <see cref="IAsyncCommandHandler{TCommand}"/>
    /// to be logged using the <see cref="IDataLogContext{TLogEntity}"/>.
    /// You need to register the expected behavior using the appropriate add data log context extension method and provide an implementation of <see cref="ILogger"/>.
    /// </summary>
    public interface ILoggingDecorator { }
}
