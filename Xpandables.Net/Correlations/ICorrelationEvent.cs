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

namespace Xpandables.Net.Correlations;

/// <summary>
/// Represents a method signature to be used to handle post event in correlation context <see cref="ICorrelationEvent"/>.
/// </summary>
/// <param name="returnValue">The control flow return value only for non-void method.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "ET001:Type name does not match file name", Justification = "<Pending>")]
public delegate Task CorrelationPostEvent(object? returnValue = default);

/// <summary>
/// Represents a method signature to be used to handler rollback event in correlation context <see cref="ICorrelationEvent"/>.
/// </summary>
/// <param name="exception">The control flow handled exception.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "ET001:Type name does not match file name", Justification = "<Pending>")]
public delegate Task CorrelationRollbackEvent(Exception exception);

/// <summary>
/// Defines two tasks that can be used to follow process after a control flow with <see cref="PostEvent"/>
/// and on exception during the control flow with <see cref="RollbackEvent"/>.
/// In order to be activated, the target class should implement the <see cref="ICorrelationDecorator"/> interface,
/// the target handling class should reference the current interface (to set the action).
/// </summary>
public interface ICorrelationEvent
{
    /// <summary>
    /// The event that will be asynchronously raised after the main one in the same control flow only if there is no exception.
    /// The event will contain the control flow "return value" for non-void method.
    /// Note that the event will be automatically removed after execution.
    /// </summary>
    event CorrelationPostEvent PostEvent;

    /// <summary>
    /// The event that will be asynchronously raised after the main one when exception. The event will contain the control flow handled exception.
    /// Note that the event will be automatically removed after execution.
    /// </summary>
    event CorrelationRollbackEvent RollbackEvent;
}

/// <summary>
/// The implementation of <see cref="ICorrelationEvent"/>.
/// Defines two tasks that can be used to follow process after a control flow with <see cref="PostEvent"/>
/// and on exception during the control flow with <see cref="RollbackEvent"/>.
/// </summary>
public sealed class CorrelationEvent : ICorrelationEvent
{
    /// <summary>
    /// Initializes the default instance of the <see cref="CorrelationEvent"/> class using default initialization for event.
    /// </summary>
    public CorrelationEvent() { }

    ///<inheritdoc/>
    public event CorrelationPostEvent PostEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);

    ///<inheritdoc/>
    public event CorrelationRollbackEvent RollbackEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);

    /// <summary>
    /// Raises the <see cref="PostEvent"/> event.
    /// </summary>
    /// <param name="returnValue">The control flow return value only for non-void method.</param>
    internal async Task OnPostEventAsync(object? returnValue = default)
    {
        try
        {
            await PostEvent(returnValue).ConfigureAwait(false);
        }
        finally
        {
            PostEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Raises the <see cref="RollbackEvent"/> event.
    /// </summary>
    /// <param name="exception">The control flow handled exception.</param>
    internal async Task OnRollbackEventAsync(Exception exception)
    {
        try
        {
            await RollbackEvent(exception).ConfigureAwait(false);
        }
        finally
        {
            RollbackEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
