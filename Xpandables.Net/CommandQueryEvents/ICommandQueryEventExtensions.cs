
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
    /// Provides with extension methods to access <see cref="ICommandQueryEvent"/> members.
    /// </summary>
    public static class ICommandQueryEventExtensions
    {
        /// <summary>
        /// Returns the <see cref="System.Guid"/> of the current object.
        /// </summary>
        /// <param name="this">The target object.</param>
        /// <returns>A <see cref="System.Guid"/> value.</returns>
        public static Guid Guid(this ICommandQueryEvent @this) => @this.Guid;

        /// <summary>
        /// Returns the <see cref="DateTimeOffset"/> of the current object.
        /// </summary>
        /// <param name="this">The target object.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value.</returns>
        public static DateTimeOffset CreatedOn(this ICommandQueryEvent @this) => @this.OccurredOn;

        /// <summary>
        /// Returns the name of the user running associated with the current object.
        /// </summary>
        /// <param name="this">The target object.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string CreatedBy(this ICommandQueryEvent @this) => @this.CreatedBy;
    }
}
