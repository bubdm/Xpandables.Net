using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel.Composition;

using Xpandables.Net5.DependencyInjection;
using Xpandables.Net5.EntityFramework;
using Xpandables.Net5.ManagedExtensibility;

namespace Xpandables.Samples.Infrastructure
{
    [Export(typeof(IAddServiceExport))]
    public sealed class ServiceRegistration : AddServiceExportExtended
    {
        public override void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataContextSettings>(configuration.GetSection(nameof(DataContextSettings)));
            services.AddXStringGeneratorCryptography();
            services.AddXInstanceCreator();
            services.AddXHttpTokenAccessor();
            services.AddXDataContext<XpandablesContextProvider>();
            services.AddXSeedBehavior<XpandablesContextInitializer, XpandablesContext>();
        }
    }
}
