﻿
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
using System;
using System.Threading.Tasks;

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// Default implementation of <see cref="IAsyncCorrelationContext"/>.
    /// This class must be used through a behavior and must be registered as follow :
    /// <code>
    ///     services.AddScoped{CorrelationContext};
    ///     services.AddScoped{ICorrelationContext}(provider=>provider.GetRequiredService{CorrelationContext}());
    /// </code>
    /// </summary>
    public sealed class AsyncCorrelationContext : IAsyncCorrelationContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncCorrelationContext"/>.
        /// </summary>
        public AsyncCorrelationContext() { }

        /// <summary>
        /// The event that will be asynchronously raised after the main one in the same control flow only if there is no exception.
        /// The event will received the control flow return value for non-void method.
        /// </summary>
        public event AsyncCorrelationPostEvent PostEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// The event that will be asynchronously raised after the main one when exception. The event will received the control flow handled exception.
        /// </summary>
        public event AsyncCorrelationRollbackEvent RollbackEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);

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
                Reset("post");
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
                Reset("rollback");
            }
        }

        /// <summary>
        /// Clears the event.
        /// </summary>
        /// <param name="event">The event to reset.</param>
        private void Reset(string @event = "post")
        {
            if (@event == "post") PostEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);
            if (@event == "rollback") RollbackEvent = async _ => await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
