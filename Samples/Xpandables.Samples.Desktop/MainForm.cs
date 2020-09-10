using Microsoft.Extensions.DependencyInjection;

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Xpandables.Net.Windows.Forms;
using Xpandables.Samples.Desktop.Models;
using Xpandables.Samples.Desktop.Views;

namespace Xpandables.Samples.Desktop
{
    public partial class MainForm : ViewData<MainFormModel>
    {
        private readonly IServiceProvider _serviceProvider;
        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            Binding(Token, ctrl => ctrl.Text, data => data.Token);
            Binding(LastName, ctrl => ctrl.Text, data => data.LastName);
            Binding(FirstName, ctrl => ctrl.Text, data => data.FirstName);
            Binding(Email, ctrl => ctrl.Text, data => data.Email);
            Binding(PictureInfo, ctrl => ctrl.Text, data => data.PictureInfo);
            Binding(Picture, ctrl => ctrl.Image, data => data.Picture);
            Binding(Gender, ctrl => ctrl.Text, data => data.Gender);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var loginForm = _serviceProvider.GetRequiredService<LoginForm>();
            if (loginForm.ShowDialog(this) != DialogResult.OK)
            {
                Close();
            }
            else
            {
                Data.Token = loginForm.SignInResponse?.Token;
                //Data.FirstName = loginForm.SignInResponse?.FirstName;
                //Data.LastName = loginForm.SignInResponse?.LastName;
                Data.Email = loginForm.SignInResponse?.Email;
                //Data.Gender = loginForm.SignInResponse.Gender;
            }
        }

        private void BtnPictureUpdate_Click(object sender, EventArgs e)
        {
            if (PictureOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Data.Picture = Image.FromFile(PictureOpenFileDialog.FileName);
                Data.PictureInfo =
                    new StringBuilder()
                        .Append("File : ").AppendLine(PictureOpenFileDialog.FileName)
                        .Append("Size : ").AppendLine($"{Data.Picture.Size}")
                        .ToString();
            }
        }
    }
}
