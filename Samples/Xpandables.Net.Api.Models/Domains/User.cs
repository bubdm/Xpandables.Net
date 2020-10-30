
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

namespace Xpandables.Net.Api.Models.Domains
{
    sealed record Encrypted(string Key, string Salt, string Value);
    sealed record Picture(string Title, byte[] Content, int Height, int Width, string Extension);

    [Table(nameof(User))]
    public sealed class User : Model
    {
        public static User Create(PhoneNumber phone, ValueEncrypted password, EmailAddress email)
            => new User(phone, email, password, Role.User, ValuePicture.Default());

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
            Password = password;
            AddEventLog(nameof(ValueEncrypted), reason);
        }

        public void ChangePicture(ValuePicture picture) => Picture = picture;

        public void RemovePicture() => Picture.Clear();

        private User(PhoneNumber phone, EmailAddress email, ValueEncrypted password, Role role, ValuePicture picture)
                : this(role)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password;
            Picture = picture;
        }

        private User(Role role)
        {
            Phone = default!;
            Email = default!;
            _password = default!;
            Role = role ?? throw new ArgumentNullException(nameof(role));
            _picture = ValuePicture.Default();
        }

        [Required]
        public PhoneNumber Phone { get; private set; }
        [Required]
        public EmailAddress Email { get; private set; }

        private Encrypted _password;

        [Required]
        public ValueEncrypted Password => new ValueEncrypted(_password.Key, _password.Value, _password.Salt);
        [Required]
        public Role Role { get; private set; }

        private Picture _picture;
        [Required]
        public ValuePicture Picture => new ValuePicture(_picture.Title, _picture.Content, _picture.Height, _picture.Width, _picture.Extension);
    }
}
