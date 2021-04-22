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

namespace Xpandables.Net.Enqueues
{
    /// <summary>
    /// Provides with extension methods to access <see cref="IEnqueueCommand"/> members.
    /// </summary>
    public static class IEnqueueCommandExtensions
    {
        /// <summary>
        /// Returns the status of the current enqueue command.
        /// </summary>
        /// <param name="this">The target command.</param>
        /// <returns>A <see cref="EnqueueStatus"/> value.</returns>
        public static EnqueueStatus Status(this IEnqueueCommand @this) => @this.Status;
    }
}
