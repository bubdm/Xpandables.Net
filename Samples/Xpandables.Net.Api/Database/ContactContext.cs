
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

using Xpandables.Net.Database;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Api.Database
{
    public sealed class ContactContext : DataContextEFCore
    {
        public ContactContext(DbContextOptions<ContactContext> contextOptions) : base(contextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AggregateEventEntityTypeConfiguration());
        }
        public DbSet<AggregateEventEntity> Events { get; set; } = default!;
    }

    public sealed class ContactContextSecond : DataContextEFCore
    {
        public ContactContextSecond(DbContextOptions<ContactContextSecond> contextOptions) : base(contextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregateEventEntity>().HasKey(new string[] { nameof(AggregateEventEntity.Id) });
            modelBuilder.Entity<AggregateEventEntity>().HasIndex(new string[] { nameof(AggregateEventEntity.Id) }).IsUnique();
        }

        public DbSet<AggregateEventEntity> Events { get; set; } = default!;
    }
}
