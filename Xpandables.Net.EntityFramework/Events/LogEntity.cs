
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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using Serilog.Events;
using Serilog.Formatting.Json;

using Xpandables.Net.Events;

namespace Xpandables.Net.EntityFramework.Events
{
    /// <summary>
    /// Helper class to implement <see cref="ILogEntity{T}"/>.
    /// <para>You must derive from this class to add custom behavior to the event log.</para>
    /// </summary>
    /// <typeparam name="T">The type of derived class.</typeparam>
    public abstract class LogEntity<T> : Entity, ILogEntity<T>
        where T : LogEntity<T>, new()
    {
        /// <summary>
        /// Initialize the logger event with default value.
        /// </summary>
        protected LogEntity()
        {
            Exception = string.Empty;
            Level = string.Empty;
            Message = string.Empty;
            MessageTemplate = string.Empty;
            TimeSpan = new DateTimeOffset(DateTime.Now);
            Properties = string.Empty;
        }

        /// <summary>
        /// Returns a unique identifier for log.
        /// </summary>
        /// <returns></returns>
        protected override string KeyGenerator()
        {
            var stringBuilder = new StringBuilder();

            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(_ => Guid.NewGuid())
                .Take(32)
                .ToList()
                .ForEach(e => stringBuilder.Append(e));

            return stringBuilder.ToString().ToUpperInvariant();
        }

        /// <summary>
        /// Contains the event identifier.
        /// </summary>
        public string? EventId { get; private set; }

        /// <summary>
        /// Contains the source context.
        /// </summary>
        public string? SourceContext { get; private set; }

        /// <summary>
        /// Contains the action identifier.
        /// </summary>
        public string? ActionId { get; private set; }

        /// <summary>
        /// Contains the action name.
        /// </summary>
        public string? ActionName { get; private set; }

        /// <summary>
        /// Contains the controller name.
        /// </summary>
        public string? ControllerName { get; private set; }

        /// <summary>
        /// Contains the request identifier.
        /// </summary>
        public string? RequestId { get; private set; }

        /// <summary>
        /// Contains the request path.
        /// </summary>
        public string? RequestPath { get; private set; }

        /// <summary>
        /// Contains the handled exception.
        /// </summary>
        public string Exception { get; private set; }

        /// <summary>
        /// Contains the event level.
        /// </summary>
        [Required, StringLength(128, MinimumLength = 3)]
        public string Level { get; private set; }

        /// <summary>
        /// Contains the event message.
        /// </summary>
        [Required]
        public string Message { get; private set; }

        /// <summary>
        /// Contains the event message template.
        /// </summary>
        public string MessageTemplate { get; private set; }

        /// <summary>
        /// Contains the event time span.
        /// </summary>
        [Required]
        public DateTimeOffset TimeSpan { get; private set; }

        /// <summary>
        /// Contains the properties in JSON format.
        /// </summary>
        public string? Properties { get; private set; }

        /// <summary>
        /// Adds an event id to the underlying instance.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        public T WithEventId(string? eventId)
        {
            EventId = eventId;
            return (T)this;
        }

        /// <summary>
        /// Adds a source context to the underlying instance.
        /// </summary>
        /// <param name="sourceContext">The source context.</param>
        public T WithSourceContext(string? sourceContext)
        {
            SourceContext = sourceContext;
            return (T)this;
        }

        /// <summary>
        /// Adds an action identifier to the underlying instance.
        /// </summary>
        /// <param name="actionId">The action identifier.</param>
        public T WithActionId(string? actionId)
        {
            ActionId = actionId;
            return (T)this;
        }

        /// <summary>
        /// Adds a source context to the underlying instance.
        /// </summary>
        /// <param name="actionName">The action name.</param>
        public T WithActionName(string? actionName)
        {
            ActionName = actionName;
            return (T)this;
        }

        /// <summary>
        /// Adds a controller name to the underlying instance.
        /// </summary>
        /// <param name="controllerName">The controller name.</param>
        public T WithControllerName(string? controllerName)
        {
            ControllerName = controllerName;
            return (T)this;
        }

        /// <summary>
        /// Adds a request identifier to the underlying instance.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        public T WithRequestId(string? requestId)
        {
            RequestId = requestId;
            return (T)this;
        }

        /// <summary>
        /// Adds a request path to the underlying instance.
        /// </summary>
        /// <param name="requestPath">The request path.</param>
        public T WithRequestPath(string? requestPath)
        {
            RequestPath = requestPath;
            return (T)this;
        }

        /// <summary>
        /// Adds a message to the underlying instance.
        /// </summary>
        /// <param name="message">The event message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public T WithMessage(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            return (T)this;
        }

        /// <summary>
        /// Adds a message template to the underlying instance.
        /// </summary>
        /// <param name="messageTemplate">the event message template.</param>
        public T WithMessageTemplate(string? messageTemplate)
        {
            MessageTemplate = messageTemplate ?? string.Empty;
            return (T)this;
        }

        /// <summary>
        /// Adds a time span to the underlying instance.
        /// </summary>
        /// <param name="dateTimeOffset">The event time span.</param>
        public T WithTimeSpan(DateTimeOffset dateTimeOffset)
        {
            TimeSpan = dateTimeOffset;
            return (T)this;
        }

        /// <summary>
        /// Adds an exception to the underlying instance.
        /// </summary>
        /// <param name="exception">The event exception.</param>
        public T WithException(Exception exception)
        {
            Exception = exception?.ToString() ?? string.Empty;
            return (T)this;
        }

        /// <summary>
        /// Adds a level to the underlying instance.
        /// </summary>
        /// <param name="level">The event level.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="level"/> is null.</exception>
        public T WithLevel(string level)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            return (T)this;
        }

        /// <summary>
        /// Adds a JSON object to the underlying instance.
        /// </summary>
        /// <param name="properties">The JSON properties</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public T WithProperties(string? properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            return (T)this;
        }

        /// <summary>
        /// Loads the underlying instance from the event.
        /// You must override this to match your requirement.
        /// </summary>
        /// <param name="logEvent">The event source.</param>
        public virtual T LoadFrom(object logEvent)
        {
            if (logEvent is not LogEvent log)
                throw new ArgumentNullException(nameof(logEvent));

            var json = ConvertLogEventToJson(log);
            var jobject = JObject.Parse(json);
            var properties = jobject.Property("Properties");

            return
                WithException(log.Exception)
                .WithLevel(log.Level.ToString())
                .WithEventId(GetProperty("EventId"))
                .WithSourceContext(GetProperty("SourceContext"))
                .WithActionId(GetProperty("ActionId"))
                .WithActionName(GetProperty("ActionName"))
                .WithControllerName(GetProperty("ControllerName"))
                .WithRequestId(GetProperty("RequestId"))
                .WithRequestPath(GetProperty("RequestPath"))
                .WithProperties(properties?.ToString())
                .WithMessage(log.RenderMessage())
                .WithMessageTemplate(log.MessageTemplate?.ToString())
                .WithTimeSpan(log.Timestamp);

            string? GetProperty(string key) => log.Properties.TryGetValue(key, out var value) ? value.ToString() : default;

            static string ConvertLogEventToJson(LogEvent log)
            {
                var stringBuilder = new StringBuilder();
                using (var writer = new StringWriter(stringBuilder))
                    new JsonFormatter().Format(log, writer);

                return stringBuilder.ToString();
            }
        }
    }
}
