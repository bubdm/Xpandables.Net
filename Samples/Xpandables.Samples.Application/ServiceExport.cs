using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

using Xpandables.Net5.DependencyInjection;
using Xpandables.Net5.Helpers;
using Xpandables.Net5.ManagedExtensibility;
using Xpandables.Samples.Application.Services;

namespace Xpandables.Samples.Application
{
    [Export(typeof(IAddServiceExport))]
    public sealed class ServiceExport : AddServiceExportExtended
    {
        public override void AddServices(IServiceCollection services, IConfiguration configuration)
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

            services.AddScoped<HttpIPService>();

            // interceptor
            //services.AddTransient<SignUpChangeFirstNameInterceptor>();
            //services.AddXInterceptor<SignUpChangeFirstNameInterceptor>(true, _ => true, assemblies);
            
        }
    }
}
