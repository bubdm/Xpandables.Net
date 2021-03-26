
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
using System.Collections.Generic;
using System.Security.Claims;

namespace Xpandables.Net.Correlations
{
    /// <summary>
    /// Provides with properties that uniquely identify a control flow process.
    /// </summary>
    public interface ICorrelationContext
    {
        /// <summary>
        /// Gets the default correlation context header name.
        /// </summary>
        public const string DefaultHeader = "X-Correlation-ID";

        /// <summary>
        /// Gets a collection of claims for the current context.
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// Gets the collection of objects for the current context.
        /// </summary>
        CorrelationCollection<string, object> Objects { get; }

        /// <summary>
        /// Gets the user identifier for the current context.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets the unique correlation identifier for the current context.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Determines whether or not correlation properties are available.
        /// </summary>
        bool IsAvailable { get; }
    }
}
