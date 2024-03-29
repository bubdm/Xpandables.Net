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

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents the unique identifier for an aggregate.
    /// </summary>
    [JsonInterfaceConverter(typeof(JsonInterfaceConverter<IAggregateId>))]
    public interface IAggregateId : IUniqueKey<Guid>
    {
        /// <summary>
        /// Returns a value that determines whether or not the aggregate identity is defined or empty.
        /// </summary>
        /// <returns><see langword="true"/> if it's defined, otherwise <see langword="false"/>.</returns>
        public virtual new bool IsEmpty() => Value == Guid.Empty;
        bool IUniqueKey<Guid>.IsEmpty() => IsEmpty();
    }
}
