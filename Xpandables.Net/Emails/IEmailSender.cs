
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Provides with a method to send emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Asynchronously sends the specified message via mail.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task SendEmailAsync(IEmailEvent message, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides with a method to send emails.
    /// </summary>
    /// <typeparam name="TEmailMessage">The type of the email  message content.</typeparam>
    public interface IEmailSender<TEmailMessage> : IEmailSender
        where TEmailMessage : notnull
    {
        /// <summary>
        /// Asynchronously sends the specified message via mail.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task SendEmailAsync(IEmailEvent<TEmailMessage> message, CancellationToken cancellationToken = default);
        Task IEmailSender.SendEmailAsync(IEmailEvent message, CancellationToken cancellationToken)
            => SendEmailAsync((IEmailEvent<TEmailMessage>)message, cancellationToken);
    }
}
