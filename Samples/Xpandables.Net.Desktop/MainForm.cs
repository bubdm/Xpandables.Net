
using System;
using System.Windows.Forms;

using Xpandables.Net.Desktop.Models;
using Xpandables.Net.Windows.Forms;
using Xpandables.Samples.Desktop.Views;

namespace Xpandables.Samples.Desktop
{
    public partial class MainForm : ViewModelDataBinding<MainFormViewModel>
    {
        private readonly LoginForm _loginForm;
        public MainForm(MainFormViewModel viewModel, LoginForm loginForm) : base(viewModel)
        {
            InitializeComponent();

            _loginForm = loginForm;

            Binding(Token, ctrl => ctrl.Text, data => data.Token);
            Binding(LastName, ctrl => ctrl.Text, data => data.LastName);
            Binding(FirstName, ctrl => ctrl.Text, data => data.FirstName);
            Binding(Email, ctrl => ctrl.Text, data => data.Phone);
            Binding(PictureInfo, ctrl => ctrl.Text, data => data.PictureInfo);
            Binding(Picture, ctrl => ctrl.Image, data => data.Picture);
            Binding(Gender, ctrl => ctrl.Text, data => data.Gender);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            if (_loginForm.ShowDialog(this) != DialogResult.OK)
            {
                Close();
                return;
            }

            await ViewModel.UpdateInfoAsync(_loginForm.LoginResponse);
        }

        private void BtnPictureUpdate_Click(object sender, EventArgs e)
        {
            if (PictureOpenFileDialog.ShowDialog() == DialogResult.OK)
                ViewModel.UpdatePicture(PictureOpenFileDialog.FileName);
        }
    }
}
