
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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Adds creation information to an entity.
    /// </summary>
    public interface IEntityCreate
    {
        /// <summary>
        /// Gets the creation date of the underlying instance. This property is automatically set by the <see cref="Created"/>.
        /// </summary>
        DateTime CreatedOn { get; }

        /// <summary>
        /// Sets the creation date time for underlying instance.
        /// </summary>
        void Created();

        /// <summary>
        /// Determines whether the instance is a new one.
        /// </summary>
        bool IsNew { get; }
    }
}
