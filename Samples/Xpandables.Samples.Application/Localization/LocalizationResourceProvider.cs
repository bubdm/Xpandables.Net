using System;
using System.Collections.Generic;

using Xpandables.Net.Localization;
using Xpandables.Net.Localization.Validations;

namespace Xpandables.Samples.Business.Localization
{
    public sealed class LocalizationResourceProvider : ILocalizationResourceProvider
    {
        public Type ValidationType => typeof(DataAnnotations);

        public IEnumerable<Type> ViewModelResourceTypes => Array.Empty<Type>();
    }
}
