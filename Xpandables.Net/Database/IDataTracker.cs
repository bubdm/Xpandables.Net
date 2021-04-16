﻿/************************************************************************************************************
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

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Provides with a property to determines tracking of entities.
    /// </summary>
    public interface IDataTracker
    {
        /// <summary>
        /// Determines whether or not the result of a query should be tracked for changes.
        /// The default value is <see langword="false"/>.
        /// </summary>
        internal bool IsTracked { get; set; }
    }
}
