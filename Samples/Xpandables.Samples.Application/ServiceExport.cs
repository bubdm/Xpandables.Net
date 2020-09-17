using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Options;
using Xpandables.Net.DependencyInjection;
using Xpandables.Net.Enumerables;
using Xpandables.Net.Extensibility;
using Xpandables.Net.Identities;
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

            services.AddXCommandQueriesHandlers(assemblies, options =>
            {
                options.UsePersistenceDecorator();
                options.UseValidatorDecorator();
                options.UseIdentityDecorator<IdentityDataProvider>();
                options.UseLoggingDecorator<HttpLogginService>();
            });

            services.AddXHttpTokenAccessor();
            services.AddXHttpHeaderAccessorExtended();
            services.AddXHttpTokenEngine<HttpTokenEngine>();

            services.AddXHttpRestClientGeoLocationHandler();
            services.AddXHttpRestClientIPLocationHandler();
            services.AddScoped<HttpIPService>();

            // database context sql access
            var dataConnection = new DataConnectionBuilder()
                .AddConnectionString("Server = (localdb)\\mssqllocaldb; Database = XSamples; Trusted_Connection = True; MultipleActiveResultSets = true")
                .AddPoolName("LocalDb")
                .EnableIntegratedSecurity()
                .Build();

            var dataOptions = new DataOptionsBuilder()
                .AddExceptionEvent(exception => Trace.WriteLine(exception))
                .Build();

            services.AddXDataBase(options =>
            {
                options.UseDataConnection(dataConnection);
                options.UseDataOptions(dataOptions);
            });

            // interceptor
            services.AddTransient<SignInRequestInterceptor>();
            //services.AddXInterceptor<SignInRequestInterceptor>(true, _ => true, assemblies);

        }
    }
}
