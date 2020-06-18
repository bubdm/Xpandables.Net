
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
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// Defines a set of methods to automatically raise <see cref="IEvent"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}"/> and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <param name="eventSource">The event to be dispatched</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource"/> is null.</exception>
        Task RaiseEventAsync(IEvent eventSource);

        /// <summary>
        /// Resolves all types that matches the <see cref="IEventHandler{TEvent}"/>
        /// and calls their handlers asynchronously.
        /// The operation will wait for all handlers to be completed.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="eventSource">The event to be dispatched</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource"/> is null.</exception>
        Task RaiseEventAsync<TEvent>(TEvent eventSource) where TEvent : class, IEvent;
    }
}