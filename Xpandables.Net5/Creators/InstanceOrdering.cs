
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
using System.Linq.Expressions;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Specified the dynamic ordering.
    /// </summary>
    class InstanceOrdering
    {
        /// <summary>
        /// Gets or sets the expression selector.
        /// </summary>
        public Expression? Selector;

        /// <summary>
        /// Gets or sets whether the ordering is ascending.
        /// </summary>
        public bool Ascending;
    }
}
