using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Data;
using Xpandables.Net.DependencyInjection;
using Xpandables.Net.Helpers;
using Xpandables.Net.ManagedExtensibility;
using Xpandables.Samples.Business.Interceptors;
using Xpandables.Samples.Business.Services;

namespace Xpandables.Samples.Business
{
    [Export(typeof(IAddServiceExport))]
    public sealed class ServiceExport : IAddServiceExport
    {
        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = typeof(SignInRequestInterceptor).Assembly.SingleToEnumerable().ToArray();

            services.Configure<DataConnection>(configuration.GetSection(nameof(DataConnection)));

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

            // database context sql access
            services.AddXDataBase<DataConnectionProvider>();

            // interceptor
            services.AddTransient<SignInRequestInterceptor>();
            services.AddXInterceptor<SignInRequestInterceptor>(true, _ => true, assemblies);

        }
    }
}
