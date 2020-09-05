using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.DependencyInjection;
using Xpandables.Net.Extensibility;
using Xpandables.Net.Extensions;
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

            //services.Configure<DataConnection>(configuration.GetSection(nameof(DataConnection)));

            services.AddXCommandQueriesHandlers(options =>
            {
                options.UsePersistenceDecorator();
                options.UseValidatorBehavior();
                options.UseIdentityDecorator<IdentityDataProvider>();
                options.UseLoggingDecorator<HttpLogginService>();
            }, assemblies);

            services.AddXHttpTokenContainer();
            services.AddXHttpTokenEngine<HttpTokenEngine>();

            services.AddXHttpRestClientGeoLocationHandler();
            services.AddXHttpRestClientIPLocationHandler();
            services.AddScoped<HttpIPService>();

            // database context sql access
            services.AddXDataBase();

            // interceptor
            services.AddTransient<SignInRequestInterceptor>();
            services.AddXInterceptor<SignInRequestInterceptor>(true, _ => true, assemblies);

        }
    }
}
