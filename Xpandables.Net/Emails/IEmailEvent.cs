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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as email event holding 
    /// a message type.
    /// </summary>
    public interface IEmailEvent : IEvent
    {
        /// <summary>
        /// Gets the target email message.
        /// </summary>
        object EmailMessage { get; }
    }

    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as email event holding 
    /// a specific <typeparamref name="TEmailMessage"/> type.
    /// </summary>
    /// <typeparam name="TEmailMessage">the type of the email.</typeparam>
    public interface IEmailEvent<out TEmailMessage> : IEmailEvent
        where TEmailMessage : notnull
    {
        /// <summary>
        /// Gets the target email message.
        /// </summary>
        new TEmailMessage EmailMessage { get; }
    }
}
