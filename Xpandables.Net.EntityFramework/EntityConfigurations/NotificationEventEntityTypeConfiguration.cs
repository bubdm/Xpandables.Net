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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Xpandables.Net.Aggregates;

namespace Xpandables.Net.Database.EntityConfigurations
{
    /// <summary>
    /// EFCore configuration for <see cref="NotificationEntity"/>.
    /// </summary>
    public sealed class NotificationEventEntityTypeConfiguration : IEntityTypeConfiguration<NotificationEntity>
    {
        ///<inheritdoc/>
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Id)
                .IsUnique();

            builder.Property(p => p.Data);
            builder.Property(p => p.IsJson);
            builder.Property(p => p.TypeFullName);
        }
    }
}
