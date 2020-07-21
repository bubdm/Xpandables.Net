
using Xpandables.Net.Notifications;

namespace Xpandables.Samples.Desktop.Models
{
    public sealed class LoginFormModel : NotifyPropertyChanged<LoginFormModel>
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
