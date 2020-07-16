using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xpandables.Net5.Helpers;
using Xpandables.Net5.HttpRestClient;
using Xpandables.Net5.Notifications;
using Xpandables.Net5.Windows.Forms;
using Xpandables.Samples.Business.Contracts;

namespace Xpandables.Samples.Desktop.Views
{
    public partial class LoginForm : Form
    {
        private readonly DynamicDataBinding<Data> dynamicData = new DynamicDataBinding<Data>(new Data());
        private readonly IHttpRestClientHandler _clientHandler;
        public LoginForm(IHttpRestClientHandler clientHandler)
        {
            InitializeComponent();

            _clientHandler = clientHandler;
            _clientHandler.Initialize("ApiClient");

            dynamicData.Binding(Email, ctrl => ctrl.Text, data => data.Email);
            dynamicData.Binding(Password, ctrl => ctrl.Text, data => data.Password);
            dynamicData.Binding(Token, ctrl => ctrl.Text, data => data.Token);
            dynamicData.Binding(BtnConnect, ctrl => ctrl.Enabled, data => data.IsBusy, value => !value);
        }

        private async void BtnConnect_Click(object sender, System.EventArgs e)
        {
            dynamicData.Source.Token = "Login...";
            dynamicData.Source.IsBusy = true;
            var request = new SignInRequest(dynamicData.Source.Email, dynamicData.Source.Password);
            var response = await _clientHandler.HandleAsync(request).ConfigureAwait(true);
            dynamicData.Source.Token = string.Empty;
            dynamicData.Source.IsBusy = false;

            if (response.IsValid())
            {
                dynamicData.Source.Token = response.Result.Token;
            }
            else
            {
                if (response.Exception.ContentIsHttpRestClientValidation() is { } modelResult)
                {
                    dynamicData.Source.Token = modelResult.Values.SelectMany(model => model).StringJoin(Environment.NewLine);
                }
                else
                {
                    dynamicData.Source.Token = response.Exception?.Message;
                }
            }
        }
    }

    public class Data : NotifyPropertyChanged<Data>
    {
        private string email;
        private string password;
        private string token;
        private bool isBusy;

        public string Email { get => email; set => SetProperty(ref email, value); }
        public string Password { get => password; set => SetProperty(ref password, value); }
        public string Token { get => token; set => SetProperty(ref token, value); }
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
    }
}
