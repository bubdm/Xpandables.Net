
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
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Default implementation for <see cref="IInstanceCreator"/>.
    /// You can customize the behavior providing your own implementing of <see cref="IInstanceCreator"/> interface.
    /// </summary>
    public sealed class InstanceCreator : IInstanceCreator
    {
        /// <summary>
        /// Define an action that will be called in case of handled exception during a create method execution.
        /// </summary>
        public Action<ExceptionDispatchInfo>? OnException { get; }

        /// <summary>
        /// Contains the instance cache.
        /// </summary>
        public ConcurrentDictionary<string, Delegate> Cache { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="InstanceCreator"/> with the string generator.
        /// </summary>
        public InstanceCreator() => (Cache, OnException) = (new ConcurrentDictionary<string, Delegate>(), default);
    }
}
