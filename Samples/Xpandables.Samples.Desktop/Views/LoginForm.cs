using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Helpers;
using Xpandables.Net.Windows.Forms;
using Xpandables.Samples.Business.Contracts;
using Xpandables.Samples.Desktop.Helpers;
using Xpandables.Samples.Desktop.Models;

namespace Xpandables.Samples.Desktop.Views
{
    public partial class LoginForm : ViewData<LoginFormModel>
    {
        private readonly IHttpRestClientHandler _clientHandler;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SignInResponse SignInResponse { get; private set; }
        public LoginForm(IHttpRestClientHandler clientHandler) : base()
        {
            InitializeComponent();

            _clientHandler = clientHandler;

            Binding(Email, ctrl => ctrl.Text, data => data.Email);
            Binding(Password, ctrl => ctrl.Text, data => data.Password);
            Binding(Loading, ctrl => ctrl.Text, data => data.Loading);
            Binding(BtnConnect, ctrl => ctrl.Enabled, data => data.IsBusy, value => !value);
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            LoginErrorProvider.Clear();

            var request = new SignInRequest(Data.Email, Data.Password);
            if (Validators.Validate(request) is { } exception)
            {
                foreach (var member in exception.MemberNames)
                {
                    if (member == Email.Name) LoginErrorProvider.SetError(Email, exception.ErrorMessage);
                    if (member == Password.Name) LoginErrorProvider.SetError(Password, exception.ErrorMessage);
                }

                return;
            }

            Data.Loading = "Connecting...";
            Data.IsBusy = true;

            var response = await _clientHandler.HandleAsync(request, cancellationTokenSource.Token).ConfigureAwait(true);

            Data.Loading = string.Empty;
            Data.IsBusy = false;

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
                    Data.Loading = response.Exception?.Message;
                }
            }
        }

        private async void BtnCancel_Click(object sender, EventArgs e)
        {
            if (Data.IsBusy)
                cancellationTokenSource.Cancel();

            Close();
            await Task.CompletedTask.ConfigureAwait(true);
        }
    }
}
