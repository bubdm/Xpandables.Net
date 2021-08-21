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

namespace Xpandables.Net.Commands;

/// <summary>
/// Represents a wrapper interface that avoids use of C# dynamics with command pattern 
/// and allows type inference for <see cref="ICommandHandler{TCommand, TResult}"/>.
/// </summary>
/// <typeparam name="TResult">Type of the result.</typeparam>
public interface ICommandHandlerWrapper<TResult> : ICanHandle
{
    /// <summary>
    /// Asynchronously handles the specified command and returns a task of the result type.
    /// </summary>
    /// <param name="command">The command to act on.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
    /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
    /// <remarks>You can throw an <see cref="OperationResultException"/> also.</remarks>
    Task<IOperationResult<TResult>> HandleAsync(ICommand<TResult> command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation for <see cref="ICommandHandlerWrapper{TResult}"/>.
/// </summary>
/// <typeparam name="TCommand">Type of command.</typeparam>
/// <typeparam name="TResult">Type of result.</typeparam>
public sealed class CommandHandlerWrapper<TCommand, TResult> : ICommandHandlerWrapper<TResult>
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _decoratee;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandlerWrapper{TCommand, TResult}"/> class with the  handler to be wrapped.
    /// </summary>
    /// <param name="decoratee">The command handler instance to be wrapped.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="decoratee"/> is null.</exception>
    public CommandHandlerWrapper(ICommandHandler<TCommand, TResult> decoratee)
        => _decoratee = decoratee ?? throw new ArgumentNullException($"{decoratee} : {nameof(TCommand)}.{nameof(TResult)}");

    ///<inheritdoc/>
    public bool CanHandle(object argument) => _decoratee.CanHandle(argument);

    ///<inheritdoc/>
    public async Task<IOperationResult<TResult>> HandleAsync(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => await _decoratee.HandleAsync((TCommand)command, cancellationToken).ConfigureAwait(false);
}
