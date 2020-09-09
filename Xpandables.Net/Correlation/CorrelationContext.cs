
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

namespace Xpandables.Net.Correlation
{
    /// <summary>
    /// Implementation of <see cref="ICorrelationContext"/>.
    /// Defines two tasks that can be used to follow process after a control flow with <see cref="PostEvent"/>
    /// and on exception during the control flow with <see cref="RollbackEvent"/>.
    /// </summary>
    public sealed class CorrelationContext : ICorrelationContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CorrelationContext"/>.
        /// </summary>
        public CorrelationContext() { }

        /// <summary>
        /// The event that will be raised after the main one in the same control flow only if there is no exception.
        /// The event will received the control flow return value for non-void method.
        /// </summary>
        public event CorrelationPostEvent PostEvent = _ => { };

        /// <summary>
        /// The event that will be raised after the main one when exception. The event will received the control flow handled exception.
        /// </summary>
        public event CorrelationRollbackEvent RollbackEvent = _ => { };

        /// <summary>
        /// Raises the <see cref="PostEvent"/> event.
        /// </summary>
        /// <param name="returnValue">The control flow return value only for non-void method.</param>
        internal void OnPostEvent(object? returnValue = default)
        {
            try
            {
                PostEvent(returnValue);
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
        internal void OnRollbackEvent(Exception exception)
        {
            try
            {
                RollbackEvent(exception);
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
            if (@event == "post") PostEvent = _ => { };
            if (@event == "rollback") RollbackEvent = _ => { };
        }
    }
}