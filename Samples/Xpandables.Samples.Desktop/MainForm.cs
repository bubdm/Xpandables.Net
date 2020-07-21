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
    public partial class MainForm : Form
    {
        private readonly DynamicDataBinding<MainFormModel> dynamicData = new DynamicDataBinding<MainFormModel>(new MainFormModel());
        private readonly IServiceProvider _serviceProvider;
        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            dynamicData.Binding(Token, ctrl => ctrl.Text, data => data.Token);
            dynamicData.Binding(LastName, ctrl => ctrl.Text, data => data.LastName);
            dynamicData.Binding(FirstName, ctrl => ctrl.Text, data => data.FirstName);
            dynamicData.Binding(Email, ctrl => ctrl.Text, data => data.Email);
            dynamicData.Binding(PictureInfo, ctrl => ctrl.Text, data => data.PictureInfo);
            dynamicData.Binding(Picture, ctrl => ctrl.Image, data => data.Picture);
            dynamicData.Binding(Gender, ctrl => ctrl.Text, data => data.Gender);
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
                dynamicData.Data.Token = loginForm.SignInResponse?.Token;
                dynamicData.Data.FirstName = loginForm.SignInResponse?.FirstName;
                dynamicData.Data.LastName = loginForm.SignInResponse?.LastName;
                dynamicData.Data.Email = loginForm.SignInResponse?.Email;
                dynamicData.Data.Gender = loginForm.SignInResponse.Gender;
            }
        }

        private void BtnPictureUpdate_Click(object sender, EventArgs e)
        {
            if (PictureOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                dynamicData.Data.Picture = Image.FromFile(PictureOpenFileDialog.FileName);
                dynamicData.Data.PictureInfo =
                    new StringBuilder()
                        .Append("File : ").AppendLine(PictureOpenFileDialog.FileName)
                        .Append("Size : ").AppendLine($"{dynamicData.Data.Picture.Size}")
                        .ToString();
            }
        }
    }
}
