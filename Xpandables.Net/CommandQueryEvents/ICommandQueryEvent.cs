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
using System;

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with shared member for commands, queries and events.
    /// </summary>
    public interface ICommandQueryEvent
    {
        /// <summary>
        /// Gets the unique identifier for the command.
        /// </summary>
        public Guid Guid => Guid.NewGuid();

        /// <summary>
        /// Gets the created date of the command.
        /// </summary>
        public DateTimeOffset CreatedOn => DateTimeOffset.Now;

        /// <summary>
        /// Gets the name of the user running associated with the current command.
        /// The default value is associated with the current thread.
        /// </summary>
        public string CreatedBy => Environment.UserName;
    }
}