/************************************************************************************************************
 * Copyright (C) 2019 Francis-Black EWANE
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

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Events;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.EntityFramework
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// This is the <see langword="abstract"/> db log context class that inherits from <see cref="DbContext"/>
    /// and implements <see cref="IDataLogContext"/>.
    /// </summary>
    public abstract class DataLogContext<TLogEntity> : DataContext, IDataLogContext<TLogEntity>
        where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataLogContext{TLogEntity}"/> class
        /// using the specified options.
        /// </summary>
        /// <param name="contextOptions">The options for this context.</param>
        protected DataLogContext(DbContextOptions contextOptions)
            : base(contextOptions) { }

        /// <summary>
        /// Contains a collection of logs.
        /// </summary>
        public DbSet<TLogEntity> Logs { get; set; } = null!;

    }
}
