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

namespace Xpandables.Net;

/// <summary>
/// Provides with a method to send SMS.
/// </summary>
public interface ISmsSender
{
    /// <summary>
    /// Asynchronously sends the specified message via SMS.
    /// </summary>
    /// <param name="sms">The message instance.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
    Task<IOperationResult> SendSmsAsync(object sms, CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides with a method to send SMS using a specific type.
/// </summary>
/// <typeparam name="TSms">The type of the SMS message content.</typeparam>
public interface ISmsSender<TSms> : ISmsSender
    where TSms : class
{
    /// <summary>
    /// Asynchronously sends the specified message type via SMS.
    /// </summary>
    /// <param name="sms">The message instance.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
    Task<IOperationResult> SendSmsAsync(TSms sms, CancellationToken cancellationToken = default);
    Task<IOperationResult> ISmsSender.SendSmsAsync(object sms, CancellationToken cancellationToken) => SendSmsAsync((TSms)sms, cancellationToken);
}
