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

using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// Allows an application author to dynamically seed a data context before it's retrieved.
    /// This is useful when you need a data context not to be empty.
    /// The target data context should be decorated with the <see cref="IInitializerDecorator"/> interface.
    /// </summary>
    public interface IDataContextInitializer
    {
        /// <summary>
        /// Asynchronously initializes the specified data context as you wish.
        /// Warning : Do not throw exception from this method unless it's absolutely necessary.
        /// This method get called by the <see cref="DataContextInitializerDecorator"/>.
        /// </summary>
        /// <param name="dataContext">The data context instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        Task InitializeAsync(IDataContext dataContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the specified data context as you wish.
        /// Warning : Do not throw exception from this method unless it's absolutely necessary.
        /// This method get called by the <see cref="DataContextInitializerDecorator"/>.
        /// </summary>
        /// <param name="dataContext">The data context instance to act on.</param>
        public void Initialize(IDataContext dataContext) => AsyncExtensions.RunSync(InitializeAsync(dataContext));
    }
}