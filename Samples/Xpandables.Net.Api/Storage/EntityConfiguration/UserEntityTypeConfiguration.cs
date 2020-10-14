
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

using Xpandables.Net.Api.Models.Domains;

namespace Xpandables.Net.Api.Storage.EntityConfiguration
{
    public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(new string[] { nameof(User.Id) });
            builder.HasIndex(new string[] { nameof(User.Id) }).IsUnique();

            builder
                .OwnsOne(user => user.Phone, phone =>
                {
                    phone.Property(p => p.Value);
                })
                .OwnsOne(user => user.Password, pwd =>
                {
                    pwd.Property(p => p.Key);
                    pwd.Property(p => p.Value);
                    pwd.Property(p => p.Salt);
                })
                .OwnsOne(user => user.Picture, pic =>
                {
                    pic.Property(p => p.Content);
                    pic.Property(p => p.Extension);
                    pic.Property(p => p.Height);
                    pic.Property(p => p.Title);
                    pic.Property(p => p.Width);
                })
                .OwnsOne(user => user.Email, email =>
                {
                    email.Property(p => p.Value);
                });

            builder
                .HasMany(user => user.EventLogs)
                .WithOne();


        }
    }
}
