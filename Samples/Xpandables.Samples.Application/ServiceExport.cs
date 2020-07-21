using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

using Xpandables.Net.DependencyInjection;
using Xpandables.Net.Helpers;
using Xpandables.Net.ManagedExtensibility;
using Xpandables.Samples.Business.Services;

namespace Xpandables.Samples.Business
{
    [Export(typeof(IAddServiceExport))]
    public sealed class ServiceExport : IAddServiceExport
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = Assembly.GetExecutingAssembly().SingleToEnumerable().ToArray();

            services.AddXQueryHandlerWrapper();
            services.AddXQueryHandlers(assemblies);
            services.AddXCommandHandlers(assemblies);

            services.AddXPersistenceBehavior();
            services.AddXValidatorRules(assemblies);
            services.AddXValidatorRuleBehavior();

            services.AddXHttpTokenContainer();
            services.AddXIdentityProvider<IdentityDataProvider>();
            services.AddXIdentityBehavior();
            services.AddXHttpTokenEngine<HttpTokenEngine>();

            services.AddXHttpRestClientGeoLocationHandler();
            services.AddXHttpRestClientIPLocationHandler();
            services.AddScoped<HttpIPService>();

            // interceptor
            //services.AddTransient<SignUpChangeFirstNameInterceptor>();
            //services.AddXInterceptor<SignUpChangeFirstNameInterceptor>(true, _ => true, assemblies);

        }
    }
}
