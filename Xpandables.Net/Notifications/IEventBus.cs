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

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Defines an interface to subscribe and publish notifications.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Asynchronously pushes notifications to any subscribers.
        /// </summary>
        /// <returns>a task that represents an asynchronous operation.</returns>
        Task PushAsync();
    }
}