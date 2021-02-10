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

using Xpandables.Net.Correlations;

namespace Xpandables.Net.Decorators
{
    /// <summary>
    /// A marker interface that allows the command/query class to add correlation decorator context event after control flow.
    /// In the class handling the query or command, you should reference
    /// the <see cref="ICorrelationEvent"/> and set the <see cref="ICorrelationEvent.PostEvent"/> and/or
    /// <see cref="ICorrelationEvent.RollbackEvent"/>.
    /// <para></para>
    /// Note that <see cref="ICorrelationEvent.PostEvent"/> will be raised at the end of the control only if there is no exception,
    /// giving you access to all data still alive on the control flow and the <see cref="ICorrelationEvent.RollbackEvent"/>
    /// will only be raised when exception. The exception in that case in accessible through the
    /// <see cref="ICorrelationEvent.RollbackEvent"/>.
    /// </summary>
    public interface ICorrelationDecorator { }
}
