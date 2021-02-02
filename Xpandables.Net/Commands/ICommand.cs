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

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// This interface is used as a marker for commands when using the asynchronous command pattern.
    /// Class implementation is used with the <see cref="ICommandHandler{TCommand}"/> where
    /// "TCommand" is a class that implements <see cref="ICommand"/>.
    /// This can also be enhanced with some useful decorators.
    /// </summary>
    public interface ICommand : ICommandQueryEvent { }

    /// <summary>
    /// This interface is used as a marker for commands when using the asynchronous command pattern with a specific-type result.
    /// Class implementation is used with the <see cref="ICommandHandler{TCommand}"/> where
    /// "TCommand" is a class that implements <see cref="ICommand{TResult}"/>.
    /// This can also be enhanced with some useful decorators.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the command.</typeparam>
    public interface ICommand<out TResult> : ICommandQueryEvent { }
}