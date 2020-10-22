using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Desktop.Models;
using Xpandables.Net.Windows.Forms;

namespace Xpandables.Samples.Desktop.Views
{
    public partial class LoginForm : ViewModelDataBinding<LoginFormViewModel>
    {
        public AuthenToken LoginResponse { get; private set; }
        public LoginForm(LoginFormViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            Binding(Phone, ctrl => ctrl.Text, data => data.Phone);
            Binding(Password, ctrl => ctrl.Text, data => data.Password);
            Binding(Loading, ctrl => ctrl.Text, data => data.Loading);
            Binding(BtnConnect, ctrl => ctrl.Enabled, data => data.IsBusy, value => !value);
        }

        private void DisplayError()
        {
            if (ViewModel.Errors.TryGetValue(Phone.Name.ToUpper(), out var phoneMessage)) LoginErrorProvider.SetError(Phone, phoneMessage);
            if (ViewModel.Errors.TryGetValue(Password.Name.ToUpper(), out var pwdMessage)) LoginErrorProvider.SetError(Password, pwdMessage);
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            LoginErrorProvider.Clear();
            if (!ViewModel.TryValidate())
            {
                DisplayError();
                return;
            }

            if (await ViewModel.TryConnectAsync())
            {
                LoginResponse = ViewModel.AuthenToken;
                DialogResult = DialogResult.OK;
                Close();
            }

            DisplayError();
        }

        private async void BtnCancel_Click(object sender, EventArgs e)
        {
            ViewModel.CancelConnect();

            Close();
            await Task.CompletedTask.ConfigureAwait(true);
        }
    }
}
