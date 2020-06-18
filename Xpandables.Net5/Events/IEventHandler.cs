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
using System.Threading.Tasks;

namespace System.Design
{
    /// <summary>
    /// Allows an application author to define a handler for an event.
    /// The event must implement <see cref="IEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        ///  Asynchronously handle the event.
        /// </summary>
        /// <param name="eventSource">The event instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource"/> is null.</exception>
        Task HandleAsync(object eventSource);
    }

    /// <summary>
    /// Allows an application author to define a handler for specific type event.
    /// The event must implement <see cref="IEvent"/> interface.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TEvent">The event type to be handled.</typeparam>
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Asynchronously handle the event.
        /// </summary>
        /// <param name="eventSource">The event instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="eventSource"/> is null.</exception>
        Task HandleAsync(TEvent eventSource);
    }
}