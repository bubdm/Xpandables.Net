
using System;

namespace Xpandables.Samples.Domain.Settings
{
    public class AuthenSettings
    {
        public AuthenSettings() { }

        public AuthenSettings(int expiration, bool slidingExpiration, string cookieName, string loginPath, string logoutPath)
        {
            Expiration = expiration;
            SlidingExpiration = slidingExpiration;
            CookieName = cookieName ?? throw new ArgumentNullException(nameof(cookieName));
            LoginPath = loginPath ?? throw new ArgumentNullException(nameof(loginPath));
            LogoutPath = logoutPath ?? throw new ArgumentNullException(nameof(logoutPath));
        }

        public int Expiration { get; set; } = 45;
        public bool SlidingExpiration { get; set; } = true;
        public string CookieName { get; set; } = "xpandablescookie";
        public string LoginPath { get; set; } = "/signin";
        public string LogoutPath { get; set; } = "/signout";
    }
}
