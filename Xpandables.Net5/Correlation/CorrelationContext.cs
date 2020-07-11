
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

namespace Xpandables.Net5.Correlation
{
    /// <summary>
    /// The implementation of <see cref="ICorrelationContext"/>.
    /// This class must be used through a behavior and must be registered as follow :
    /// <code>
    ///     services.AddScoped{CorrelationContext};
    ///     services.AddScoped{ICorrelationContext}(provider=>provider.GetService{CorrelationContext}());
    /// </code>
    /// </summary>
    public sealed class CorrelationContext : ICorrelationContext
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of <see cref="CorrelationContext"/> with the service provider.
        /// </summary>
        /// <param name="provider">The service provider to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public CorrelationContext(IServiceProvider provider)
            => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// The event that will be executed after the main one in the same control flow only if there is no exception.
        /// The event will received the control flow return value for non-void method.
        /// </summary>
        public event CorrelationPostEvent PostEvent = async (_, __) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// The event that will be executed after the main one when exception. The event will received the control flow handled exception.
        /// </summary>
        public event CorrelationRollbackEvent RollbackEvent = async (_, __) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        ///  Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType or null if there is no service object of type serviceType.</returns>
        object? IServiceProvider.GetService(Type serviceType) => _provider.GetService(serviceType);

        /// <summary>
        /// Raises the <see cref="PostEvent"/> event.
        /// </summary>
        /// <param name="returnValue">The control flow return value only for non-void method.</param>
        internal async Task OnPostEventAsync(object? returnValue = default)
        {
            try
            {
                await PostEvent(_provider, returnValue).ConfigureAwait(false);
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
                await RollbackEvent(_provider, exception).ConfigureAwait(false);
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
            if (@event == "post") PostEvent = async (_, __) => await Task.CompletedTask.ConfigureAwait(false);
            if (@event == "rollback") RollbackEvent = async (_, __) => await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}