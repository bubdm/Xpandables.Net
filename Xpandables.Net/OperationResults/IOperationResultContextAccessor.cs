
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

namespace Xpandables.Net.OperationResults
{
    /// <summary>
    /// Provides with properties that uniquely identify a control flow process.
    /// </summary>
    public interface IOperationResultContextAccessor
    {
        /// <summary>
        /// Gets the user identifier for the operation context.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets the user name for the operation context.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the unique correlation identifier of the operation context.
        /// </summary>
        Guid CorrelationId { get; }

        /// <summary>
        /// Determines whether or not correlation properties are available.
        /// </summary>
        bool IsAvailable { get; }
    }
}
