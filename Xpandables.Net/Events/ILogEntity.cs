
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

using Serilog.Events;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Provides with the base description for an event log using <see cref="Serilog"/>.
    /// </summary>
    public interface ILogEntity
    {
        /// <summary>
        /// Contains log identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Contains the event identifier.
        /// </summary>
        string? EventId { get; }

        /// <summary>
        /// Contains the source context.
        /// </summary>
        string? SourceContext { get; }

        /// <summary>
        /// Contains the action identifier.
        /// </summary>
        string? ActionId { get; }

        /// <summary>
        /// Contains the action name.
        /// </summary>
        string? ActionName { get; }

        /// <summary>
        /// Contains the controller name.
        /// </summary>
        string? ControllerName { get; }

        /// <summary>
        /// Contains the request identifier.
        /// </summary>
        string? RequestId { get; }

        /// <summary>
        /// Contains the request path.
        /// </summary>
        string? RequestPath { get; }

        /// <summary>
        /// Contains the handled exception.
        /// </summary>
        string? Exception { get; }

        /// <summary>
        /// Contains the level.
        /// </summary>
        string Level { get; }

        /// <summary>
        /// Contains the message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Contains the message template.
        /// </summary>
        string MessageTemplate { get; }

        /// <summary>
        /// Contains the event time span.
        /// </summary>
        DateTimeOffset TimeSpan { get; }

        /// <summary>
        /// Contains the properties in JSON format.
        /// </summary>
        string? Properties { get; }
    }

    /// <summary>
    /// Provides with the generic base description for an event log using <see cref="Serilog"/>.
    /// </summary>
    /// <typeparam name="T">The type of the log event class.</typeparam>
    public interface ILogEntity<T> : ILogEntity
        where T : class, ILogEntity<T>, new()
    {
        /// <summary>
        /// Adds a message to the underlying instance.
        /// </summary>
        /// <param name="message">The event message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        T WithMessage(string message);

        /// <summary>
        /// Adds an event id to the underlying instance.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        T WithEventId(string? eventId);

        /// <summary>
        /// Adds a source context to the underlying instance.
        /// </summary>
        /// <param name="sourceContext">The source context.</param>
        T WithSourceContext(string? sourceContext);

        /// <summary>
        /// Adds an action identifier to the underlying instance.
        /// </summary>
        /// <param name="actionId">The action identifier.</param>
        T WithActionId(string? actionId);

        /// <summary>
        /// Adds a source context to the underlying instance.
        /// </summary>
        /// <param name="actionName">The action name.</param>
        T WithActionName(string? actionName);

        /// <summary>
        /// Adds a controller name to the underlying instance.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        T WithControllerName(string? controllerName);

        /// <summary>
        /// Adds a request identifier to the underlying instance.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        T WithRequestId(string? requestId);

        /// <summary>
        /// Adds a request path to the underlying instance.
        /// </summary>
        /// <param name="requestPath">The request path.</param>
        T WithRequestPath(string? requestPath);

        /// <summary>
        /// Adds a message template to the underlying instance.
        /// </summary>
        /// <param name="messageTemplate">the event message template.</param>
        T WithMessageTemplate(string messageTemplate);

        /// <summary>
        /// Adds a time span to the underlying instance.
        /// </summary>
        /// <param name="dateTimeOffset">The event time span.</param>
        T WithTimeSpan(DateTimeOffset dateTimeOffset);

        /// <summary>
        /// Adds an exception to the underlying instance.
        /// </summary>
        /// <param name="exception">The event exception.</param>
        T WithException(Exception exception);

        /// <summary>
        /// Adds a level to the underlying instance.
        /// </summary>
        /// <param name="level">The event level.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="level"/> is null.</exception>
        T WithLevel(string level);

        /// <summary>
        /// Adds a JSON object to the underlying instance.
        /// </summary>
        /// <param name="properties">The JSON properties</param>
        T WithProperties(string? properties);

        /// <summary>
        /// Loads the underlying instance from the <see cref="Serilog"/> event.
        /// </summary>
        /// <param name="logEvent">The event source.</param>
        T LoadFrom(LogEvent logEvent);
    }
}
