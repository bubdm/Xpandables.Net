
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Notifications;
using Xpandables.Samples.Desktop.Helpers;

namespace Xpandables.Net.Desktop.Models
{
    public sealed class LoginFormViewModel : NotifyPropertyChanged<LoginFormViewModel>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IHttpRestClientHandler _httpRestClientHandler;
        private string _phone;
        private string _password;
        private string _loading;
        private bool _isBusy;

        public LoginFormViewModel(IHttpRestClientHandler httpRestClientHandler) => _httpRestClientHandler = httpRestClientHandler ?? throw new ArgumentNullException(nameof(httpRestClientHandler));

        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Loading { get => _loading; set => SetProperty(ref _loading, value); }
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public IReadOnlyDictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();
        public AuthenToken AuthenToken { get; private set; }

        public bool TryValidate()
        {
            var request = new GetAuthenToken(Phone, Password);
            if (Validators.Validate(request) is { } exceptions)
            {
                Errors = exceptions.ToDictionary(d => d.MemberNames.First(), d => d.ErrorMessage);
                return false;
            }

            return true;
        }

        public async Task<bool> TryConnectAsync()
        {
            Loading = "Connecting...";
            IsBusy = true;

            var request = new GetAuthenToken(Phone, Password);
            var response = await _httpRestClientHandler.HandleAsync(request, cancellationTokenSource.Token).ConfigureAwait(true);

            Loading = string.Empty;
            IsBusy = false;

            if (response.IsValid())
            {
                AuthenToken = response.Result;
                return true;
            }

            if (_httpRestClientHandler.HttpRestClientEngine.IsHttpRestClientValidation(response.Exception, out var exception))
            {
                Errors = exception.ToDictionary(d => d.Key.ToUpper(), d => string.Join("-", d.Value));
            }
            else
            {
                Loading = response.Exception?.Message;
            }

            return false;
        }

        public void CancelConnect()
        {
            if (IsBusy)
                cancellationTokenSource.Cancel();
        }
    }
}
