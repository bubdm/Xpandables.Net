﻿/************************************************************************************************************
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

namespace Xpandables.Net.EntityFramework.EntityConfigurations
{
    /// <summary>
    /// EFCore configuration for <see cref="SnapShotEntity"/>.
    /// </summary>
    public sealed class SnapShotEntityTypeConfiguration : IEntityTypeConfiguration<SnapShotEntity>
    {
        ///<inheritdoc/>
        public void Configure(EntityTypeBuilder<SnapShotEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.AggregateId, p.Version })
                .IsUnique();

            builder.Property(p => p.AggregateId).IsConcurrencyToken();
            builder.Property(p => p.Data);
            builder.Property(p => p.Id);
            builder.Property(p => p.Version).IsConcurrencyToken();
            builder.Property(p => p.IsJson);
            builder.Property(p => p.Type);
        }
    }
}