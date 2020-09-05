using System;
using System.ComponentModel.Composition;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.DependencyInjection;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Events;
using Xpandables.Net.Extensibility;

namespace Xpandables.Samples.Infrastructure
{
    [Export(typeof(IAddServiceExport))]
    public sealed class ServiceRegistration : IAddServiceExport
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            services.Configure<DataContextSettings>(configuration.GetSection(nameof(DataContextSettings)));
            services.Configure<DataLogContextSettings>(configuration.GetSection(nameof(DataLogContextSettings)));
            services.AddXStringGeneratorCryptography();
            services.AddXInstanceCreator();
            services.AddXHttpTokenAccessor();
            services.AddXDataContext<XpandablesContextProvider>();
            services.AddXDataLogContext<XpandablesLogContextProvider>();
            services.AddXSeedDecorator<XpandablesContextInitializer, XpandablesContext>();
        }
    }
}
