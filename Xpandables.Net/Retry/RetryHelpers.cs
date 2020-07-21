
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
using System.Reflection;

namespace Xpandables.Net.Retry
{
    /// <summary>
    /// Provides extensions methods for behaviors.
    /// </summary>
    internal static class RetryHelpers
    {
        /// <summary>
        /// Returns the <see cref="RetryBehaviorAttribute"/> from the specified argument.
        /// </summary>
        /// <typeparam name="TSource">The type of the argument.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="serviceProvider">The service provider.</param>
        internal static RetryBehaviorAttribute GetRetryBehaviorAttribute<TSource>(this TSource source, IServiceProvider serviceProvider)
            where TSource : class
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source is IRetryBehaviorAttributeProvider retrybehaviorAttributeProvider
                ? retrybehaviorAttributeProvider.GetRetryBehaviorAttribute(serviceProvider)
                : source.GetType().GetCustomAttribute<RetryBehaviorAttribute>()
                ?? throw new RetryBehaviorException(
                    $"{nameof(RetryBehaviorAttribute)} or {nameof(IRetryBehaviorAttributeProvider)} implementation excepted from : {source.GetType().Name}");
        }
    }
}
