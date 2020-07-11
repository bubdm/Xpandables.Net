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
using System.Windows.Input;

using Xpandables.Net5.Queries;

namespace Xpandables.Net5.Retry
{
    /// <summary>
    /// A marker interface that allows the command/query class handler implementation to be decorated with the retry behavior according to
    /// the decorated class type :
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryRetryBehavior{TQuery, TResult}"/>.
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandRetryBehavior{TCommand}"/>.
    /// The command/query class must be decorated with <see cref="RetryBehaviorAttribute"/> or implement the <see cref="IRetryBehaviorAttributeProvider"/>.
    /// The handler can implement the <see cref="IRetryBehaviorHandler{TArgument}"/> to manage the retry execution.
    /// <para></para>
    /// You need to register the expected behavior to the service collections using the appropriate extension method.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IBehaviorRetry { }
#pragma warning restore CA1040 // Avoid empty interfaces
}
