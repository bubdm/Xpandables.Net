using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xpandables.Samples.Desktop.Views;

namespace Xpandables.Samples.Desktop
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var loginForm = _serviceProvider.GetRequiredService<LoginForm>();
            if (loginForm.ShowDialog(this) != DialogResult.OK)
                Close();
            else
                MessageBox.Show(loginForm.SignInResponse.Token);
        }
    }
}
