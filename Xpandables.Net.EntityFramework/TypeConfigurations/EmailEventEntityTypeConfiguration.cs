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

using Xpandables.Net.Events;

namespace Xpandables.Net.TypeConfigurations
{
    /// <summary>
    /// EFCore configuration for email event.
    /// </summary>
    public sealed class EmailEventEntityTypeConfiguration : IEntityTypeConfiguration<EmailEventStoreEntity>
    {
        ///<inheritdoc/>
        public void Configure(EntityTypeBuilder<EmailEventStoreEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.Id, p.AggregateId });

            builder.Property(p => p.AggregateId);
            builder.Property(p => p.AggregateTypeName);
            builder.Property(p => p.EventData);
            builder.Property(p => p.EventTypeFullName);
            builder.Property(p => p.EventTypeName);
            builder.Property(p => p.ExceptionTypeFullName);
            builder.Property(p => p.Exception);
            builder.Property(p => p.State);
        }
    }
}
