
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

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Api.Storage.EntityConfiguration;
using Xpandables.Net.EntityFramework;

namespace Xpandables.Net.Api.Storage
{
    public sealed class UserContext : DataContext, ISeedDecorator
    {
        public UserContext(DbContextOptions<UserContext> contextOptions)
            : base(contextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

            modelBuilder.UseEnumerationValueConverterForType(EnumerationConverter<Gender>());
            modelBuilder.UseEnumerationValueConverterForType(EnumerationConverter<Role>());
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<EventLog> EventLogs { get; set; } = default!;
    }
}
