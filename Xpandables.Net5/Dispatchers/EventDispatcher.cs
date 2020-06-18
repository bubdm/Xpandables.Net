
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
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// The default implementation for <see cref="IEventDispatcher"/>.
    /// Implements methods to execute the <see cref="IEventHandler{T}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="EventDispatcher"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public EventDispatcher(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}" />
        /// and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="eventSource">The event to be dispatched</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource" /> is null.</exception>
        public async Task RaiseEventAsync<TEvent>(TEvent eventSource) where TEvent : class, IEvent
        {
            if (eventSource is null) throw new ArgumentNullException(nameof(eventSource));

            var tasks = _serviceProvider
                .GetServices<IEventHandler<TEvent>>()
                .Select(handler => handler.HandleAsync(eventSource));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}" /> and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <param name="eventSource">The event to be dispatched</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource" /> is null.</exception>
        public async Task RaiseEventAsync(IEvent eventSource)
        {
            if (eventSource is null) throw new ArgumentNullException(nameof(eventSource));
            if (!typeof(IEventHandler<>).TryMakeGenericType(out var typeHandler, out var typeException, eventSource.GetType()))
                throw new InvalidOperationException("Building Event Handler type failed.", typeException);

            var tasks = _serviceProvider
                .GetServices(typeHandler)
                .Select(handler => ((IEventHandler)handler).HandleAsync(eventSource));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}