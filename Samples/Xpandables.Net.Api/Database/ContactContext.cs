
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

using Xpandables.Net.Entities;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.EntityFramework.Entities;
using Xpandables.Net.EntityFramework.EntityConfigurations;

namespace Xpandables.Net.Api.Database
{
    public sealed class ContactContext : DataContext
    {
        public ContactContext(DbContextOptions<ContactContext> contextOptions) : base(contextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DomainEventEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationEventEntityTypeConfiguration());
        }
        public DbSet<DomainEventEntity> Events { get; set; } = default!;
        public DbSet<IntegrationEventEntity> Integrations { get; set; } = default!;
    }

    public sealed class ContactContextSecond : DataContext
    {
        public ContactContextSecond(DbContextOptions<ContactContextSecond> contextOptions) : base(contextOptions) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DomainEventEntityTypeConfigurationNewtosoft());
            modelBuilder.Entity<DomainEventEntityNewtonsoft>().HasKey(new string[] { nameof(DomainEventEntityNewtonsoft.Id) });
            modelBuilder.Entity<DomainEventEntityNewtonsoft>().HasIndex(new string[] { nameof(DomainEventEntityNewtonsoft.Id) }).IsUnique();
        }

        public DbSet<DomainEventEntityNewtonsoft> Events { get; set; } = default!;
    }
}
