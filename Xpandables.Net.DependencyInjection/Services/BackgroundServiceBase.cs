
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
using Microsoft.Extensions.Hosting;

namespace Xpandables.Net.Services;

/// <summary>
/// Represents a helper class that allows implementation of <see cref="IBackgroundService"/>.
/// </summary>
/// <typeparam name="TBackgroundService">The type of target background service.</typeparam>
public abstract class BackgroundServiceBase<TBackgroundService> : BackgroundService, IBackgroundService
    where TBackgroundService : IBackgroundService
{
    ///<inheritdoc/>
    public bool IsRunning { get; protected set; }

    ///<inheritdoc/>
    public virtual async Task<IOperationResult> StartServiceAsync(CancellationToken cancellationToken = default)
    {
        if (IsRunning)
            return new FailureOperationResult(new OperationErrorCollection("status", $"{nameof(TBackgroundService)} is already up."));

        IsRunning = true;
        await StartAsync(cancellationToken).ConfigureAwait(false);
        return new SuccessOperationResult();
    }

    ///<inheritdoc/>
    public virtual async Task<IOperationResult> StopServiceAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
            return new FailureOperationResult(new OperationErrorCollection("status", $"{nameof(TBackgroundService)} is already down."));

        await StopAsync(cancellationToken).ConfigureAwait(false);
        IsRunning = false;
        return new SuccessOperationResult();
    }

    ///<inheritdoc/>
    public virtual async Task<IOperationResult<string>> StatusServiceAsync()
    {
        var response = $"{nameof(TBackgroundService)} {(IsRunning ? "Is Up" : "Is Down")}";
        return await Task.FromResult(new SuccessOperationResult<string>(response)).ConfigureAwait(false);
    }
}
