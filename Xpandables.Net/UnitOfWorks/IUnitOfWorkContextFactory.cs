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

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents a method to create a unit of work context.
    /// </summary>
    public interface IUnitOfWorkContextFactory
    {
        /// <summary>
        /// Creates a new instance of <typeparamref name="TUnitOfWorkContext"/> type.
        /// </summary>
        /// <typeparam name="TUnitOfWorkContext">The type of the expected context.</typeparam>
        /// <returns>An instance of <typeparamref name="TUnitOfWorkContext"/> type.</returns>
        TUnitOfWorkContext CreateUnitOfWorkContext<TUnitOfWorkContext>()
            where TUnitOfWorkContext : class, IUnitOfWorkContext;
    }
}
