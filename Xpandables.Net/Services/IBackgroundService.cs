﻿/************************************************************************************************************
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

namespace Xpandables.Net.Services;

/// <summary>
/// Provides with method to manage background service.
/// </summary>
public interface IBackgroundService
{
    /// <summary>
    /// Determines whether the service is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// tries to stop the service.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
    Task<IOperationResult> StopServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to start the service.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
    Task<IOperationResult> StartServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the status of the service.
    /// </summary>
    /// <returns>A task that represents an object of <see cref="IOperationResult{TValue}"/>.</returns>
    public virtual async Task<IOperationResult<string>> StatusServiceAsync()
    {
        var response = IsRunning ? "Is Up" : "Is Down";
        return await Task.FromResult(new SuccessOperationResult<string>(response)).ConfigureAwait(false);
    }
}
