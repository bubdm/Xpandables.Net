namespace Xpandables.Samples.Domain.Settings
{
    public sealed class ApiUrlSettings
    {
        public ApiUrlSettings() { }

#pragma warning disable CA1054 // Uri parameters should not be strings
        public ApiUrlSettings(string url)
#pragma warning restore CA1054 // Uri parameters should not be strings
        {
            Url = url;
        }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
    }
}
