using System;

namespace Xpandables.Samples.Domain.Settings
{
    public class JwtSettings
    {
        public JwtSettings() { }

#pragma warning disable CA1707 // Identifiers should not contain underscores
        public JwtSettings(string jWT_Secret, string jWT_Issuer, ushort jWT_Expiration, ushort jWT_RefreshExpiration)
        {
            JWT_Secret = jWT_Secret ?? throw new ArgumentNullException(nameof(jWT_Secret));
            JWT_Issuer = jWT_Issuer ?? throw new ArgumentNullException(nameof(jWT_Issuer));
            JWT_Expiration = jWT_Expiration;
            JWT_RefreshExpiration = jWT_RefreshExpiration;
        }

        public string JWT_Secret { get; set; } = "NONE";
        public string JWT_Issuer { get; set; } = "NONE";
        public ushort JWT_Expiration { get; set; }
        public ushort JWT_RefreshExpiration { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
    }
}
