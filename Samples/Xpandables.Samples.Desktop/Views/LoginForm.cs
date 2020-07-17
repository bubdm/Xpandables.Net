using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xpandables.Net5.Helpers;
using Xpandables.Net5.HttpRestClient;
using Xpandables.Net5.Notifications;
using Xpandables.Net5.Windows.Forms;
using Xpandables.Samples.Business.Contracts;
using Xpandables.Samples.Desktop.Helpers;

namespace Xpandables.Samples.Desktop.Views
{
    public partial class LoginForm : Form
    {
        private readonly DynamicDataBinding<Data> dynamicData = new DynamicDataBinding<Data>(new Data());
        private readonly IHttpRestClientHandler _clientHandler;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SignInResponse SignInResponse { get; private set; }
        public LoginForm(IHttpRestClientHandler clientHandler)
        {
            InitializeComponent();

            _clientHandler = clientHandler;
            _clientHandler.Initialize("ApiClient");     

            dynamicData.Binding(Email, ctrl => ctrl.Text, data => data.Email);
            dynamicData.Binding(Password, ctrl => ctrl.Text, data => data.Password);
            dynamicData.Binding(Loading, ctrl => ctrl.Text, data => data.Loading);
            dynamicData.Binding(BtnConnect, ctrl => ctrl.Enabled, data => data.IsBusy, value => !value);
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            LoginErrorProvider.Clear();

            var request = new SignInRequest(dynamicData.Source.Email, dynamicData.Source.Password);
            if (Validators.Validate(request) is { } exception)
            {
                foreach (var member in exception.MemberNames)
                {
                    if (member == Email.Name) LoginErrorProvider.SetError(Email, exception.ErrorMessage);
                    if (member == Password.Name) LoginErrorProvider.SetError(Password, exception.ErrorMessage);
                }

                return;
            }

            dynamicData.Source.Loading = "Connecting...";
            dynamicData.Source.IsBusy = true;

            var response = await _clientHandler.HandleAsync(request, cancellationTokenSource.Token).ConfigureAwait(true);

            dynamicData.Source.Loading = string.Empty;
            dynamicData.Source.IsBusy = false;

            if (response.IsValid())
            {
                SignInResponse = response.Result;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                if (response.Exception.ContentIsHttpRestClientValidation() is { } validation)
                {
                    foreach (var error in validation)
                    {
                        if (error.Key == Email.Name)
                            LoginErrorProvider.SetError(Email, error.Value.StringJoin(Environment.NewLine));

                        if (error.Key == Password.Name)
                            LoginErrorProvider.SetError(Password, error.Value.StringJoin(Environment.NewLine));
                    }
                }
                else
                {
                    dynamicData.Source.Loading = response.Exception?.Message;
                }
            }
        }

        private async void BtnCancel_Click(object sender, EventArgs e)
        {
            if (dynamicData.Source.IsBusy)
                cancellationTokenSource.Cancel();

            Close();
            await Task.CompletedTask.ConfigureAwait(true);
        }
    }

    public class Data : NotifyPropertyChanged<Data>
    {
        private string email;
        private string password;
        private string loading;
        private bool isBusy;

        public string Email { get => email; set => SetProperty(ref email, value); }
        public string Password { get => password; set => SetProperty(ref password, value); }
        public string Loading { get => loading; set => SetProperty(ref loading, value); }
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    }
}
