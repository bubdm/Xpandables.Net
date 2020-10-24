
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
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Xpandables.Net.Strings;
using Xpandables.Net.Types;

namespace Xpandables.Net.Api.Models.Domains
{
    [Table(nameof(User))]
    public sealed class User : Model
    {
        public static User Create(PhoneNumber phone, ValueEncrypted password, EmailAddress email)
            => new User(phone, email, password, Role.User, Picture.Default());

        public TokenClaims CreateTokenClaims() => new TokenClaims(Id, Phone, Role, Email);

        public void ChangePhone(PhoneNumber phone, string reason)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            AddEventLog(nameof(PhoneNumber), reason);
        }

        public void ChangeEmail(EmailAddress email, string reason)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            AddEventLog(nameof(EmailAddress), reason);
        }

        public void ChangePassword(ValueEncrypted password, string reason)
        {
            Password = password ?? throw new ArgumentNullException(nameof(password));
            AddEventLog(nameof(ValueEncrypted), reason);
        }

        public void ChangePicture(Picture picture) => Picture = picture ?? throw new ArgumentNullException(nameof(picture));

        public void RemovePicture() => Picture = Picture.Clear();

        private User(PhoneNumber phone, EmailAddress email, ValueEncrypted password, Role role, Picture picture)
                : this(role)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Picture = picture ?? throw new ArgumentNullException(nameof(picture));
        }

        private User(Role role)
        {
            Phone = default!;
            Email = default!;
            Password = default!;
            Role = role ?? throw new ArgumentNullException(nameof(role));
            Picture = Picture.Default();
        }

        [Required]
        public PhoneNumber Phone { get; private set; }
        [Required]
        public EmailAddress Email { get; private set; }
        [Required]
        public ValueEncrypted Password { get; private set; }
        [Required]
        public Role Role { get; private set; }
        [Required]
        public Picture Picture { get; private set; }
    }
}
