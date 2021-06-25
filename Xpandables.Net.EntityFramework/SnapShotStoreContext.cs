
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
using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Database.EntityConfigurations;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// The <see cref="SnapShotStoreContext"/> data context definition.
    /// </summary>
    public class SnapShotStoreContext : DataContext, ISnapShotStoreContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapShotStoreContext"/> class
        /// using the specified options. The <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>
        /// method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="contextOptions">The options for this context.</param>
        public SnapShotStoreContext(DbContextOptions contextOptions) : base(contextOptions) { }

        ///<inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventEntityTypeConfiguration());
        }

        /// <summary>
        /// Gets the snapShots collection. Read/Write
        /// </summary>
        public DbSet<EventStoreEntity> SnapShots { get; set; } = default!;
    }
}
