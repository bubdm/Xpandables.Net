
using System.Drawing;

using Xpandables.Net.Entities;
using Xpandables.Net.Notifications;

namespace Xpandables.Samples.Desktop.Models
{
    public sealed class MainFormModel : NotifyPropertyChanged<MainFormModel>
    {
        private string token;
        private string lastName;
        private string firstName;
        private string email;
        private string gender;
        private Image picture;
        private string pictureInfo;

        public string Email { get => email; set => SetProperty(ref email, value); }
        public string Token { get => token; set => SetProperty(ref token, value); }
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }
        public string Gender { get => gender; set => SetProperty(ref gender, value); }
        public Image Picture { get => picture; set => SetProperty(ref picture, value); }
        public string PictureInfo { get => pictureInfo; set => SetProperty(ref pictureInfo, value); }
    }
}
