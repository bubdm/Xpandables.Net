
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
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Notifications;

namespace Xpandables.Net.Desktop.Models
{
    public sealed class MainFormViewModel : NotifyPropertyChanged<MainFormViewModel>
    {
        private readonly IHttpRestClientHandler _httpRestClientHandler;

        private string token;
        private string lastName;
        private string firstName;
        private string phone;
        private string gender;
        private Image picture;
        private string pictureInfo;

        public MainFormViewModel(IHttpRestClientHandler httpRestClientHandler) => _httpRestClientHandler = httpRestClientHandler ?? throw new ArgumentNullException(nameof(httpRestClientHandler));

        public string Phone { get => phone; set => SetProperty(ref phone, value); }
        public string Token { get => token; set => SetProperty(ref token, value); }
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }
        public string Gender { get => gender; set => SetProperty(ref gender, value); }
        public Image Picture { get => picture; set => SetProperty(ref picture, value); }
        public string PictureInfo { get => pictureInfo; set => SetProperty(ref pictureInfo, value); }

        public void UpdatePicture(string fileName)
        {
            Picture?.Dispose();
            Picture = Image.FromFile(fileName);
            PictureInfo = new StringBuilder()
                .Append("File : ").AppendLine(fileName)
                .Append("Size : ").AppendLine($"{Picture.Size}")
                .ToString();
        }

        public async Task UpdateInfoAsync(AuthenToken authenToken)
        {
            _httpRestClientHandler.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authenToken.Type, authenToken.Token);
            var user = await _httpRestClientHandler.HandleAsync(new GetUser(authenToken.Key));
            Phone = user.Result.Phone;
            Token = authenToken.Token;

        }
    }
}
