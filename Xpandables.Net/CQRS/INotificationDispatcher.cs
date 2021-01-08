
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Defines a method to automatically handle <see cref="INotification"/>
    /// when targeting <see cref="INotificationHandler{TNotification}"/>.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface INotificationDispatcher
    {
        internal IDispatcherHandlerProvider DispatcherHandlerProvider { get; }
        
        /// <summary>
        /// Asynchronously publishes the notification across all <see cref="INotificationHandler{TNotification}"/>.
        /// </summary>
        /// <param name="notification">The notification to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        public virtual async Task PublishAsync(INotification notification, CancellationToken cancellationToken = default)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            if (!typeof(INotificationHandler<>).TryMakeGenericType(out var typeHandler, out var typeException, notification.GetType()))
            {
                WriteLineException(new InvalidOperationException("Building Notification Handler type failed.", typeException));
                return;
            }

            if (!DispatcherHandlerProvider.TryGetHandlers(typeHandler, out var foundHandlers, out var ex))
            {
                WriteLineException(new InvalidOperationException($"Matching notification handlers for {notification.GetType().Name} are missing.", ex));
                return;
            }

            var handlers = (IEnumerable<INotificationHandler>)foundHandlers;

            await Task.WhenAll(handlers.Select(handler => handler.HandleAsync(notification, cancellationToken))).ConfigureAwait(false);
        }

        private static void WriteLineException(Exception exception)
        {
#if DEBUG
            Debug.WriteLine(exception);
#else
            Trace.WriteLine(exception);
#endif
        }
    }
}