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

namespace Xpandables.Net5.Retry
{
    /// <summary>
    /// An interface representing an <see cref="RetryBehaviorAttribute"/> to be dynamically applied on the implementing class.
    /// This interface takes priority over the <see cref="RetryBehaviorAttribute"/> declaration.
    /// </summary>
    public interface IRetryBehaviorAttributeProvider
    {
        /// <summary>
        /// Returns the <see cref="RetryBehaviorAttribute"/> to be applied on the current instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <returns>An instance of a new <see cref="RetryBehaviorAttribute"/>.</returns>
        RetryBehaviorAttribute GetRetryBehaviorAttribute(IServiceProvider serviceProvider);
    }
}
