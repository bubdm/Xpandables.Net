
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using Xpandables.Net.Api.Database;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Api.Middlewares;
using Xpandables.Net.Api.Services;
using Xpandables.Net.Database;
using Xpandables.Net.DependencyInjection;

namespace Xpandables.Net.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                //.AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new OperationResultNewtonsoftConverter())) uncomment to use the Newtonsoft
                .AddMvcOptions(options =>
                {
                    options.Filters.Add<OperationResultFilter>(int.MinValue);
                    options.ModelBinderProviders.Insert(0, new FromRouteModelBinderProvider());
                });

            services
                .AddXpandableServices()
                .AddXDataContext<ContactContext>(options => options.UseSqlServer(Configuration.GetConnectionString("xpandables"))
                .EnableServiceProviderCaching())
                .AddXEventStoreDataContext(options => options.UseSqlServer(Configuration.GetConnectionString("xpandables"), builder => builder.MigrationsAssembly("Xpandables.Net.Api"))
                    .EnableServiceProviderCaching())
                .AddXDispatcher()
                .AddXHandlerAccessor()
                .AddXHandlers(new[] { typeof(ContactHandlers).Assembly }, options =>
                {
                    options.UseAggregatePersistenceDecorator();
                    options.UseOperationResultLoggerDecorator();
                    options.UseValidatorDecorator();
                })
                .AddXInstanceCreator()
                .AddXOperationResultLogger<LoggingService>()
                .AddXEventStore()
                .AddXEventStoreConverterNewtonsoft()
                .AddXDomainEventPublisher()
                .AddXNotificationPublisher()
                .AddXAggregateAccessor()
                .AddXNotificationBusService()
                .AddXEventBus()
                .AddXServiceScopeFactory()
                .AddXInterceptorHandlers<ContactInterceptor>(new[] { Assembly.GetExecutingAssembly() })
                .AddXNotificationHandlers(new[] { Assembly.GetExecutingAssembly() })
                .AddXDomainEventHandlers(new[] { Assembly.GetExecutingAssembly() })
                .AddXHttpRestClientHandler(_ => { })
                .AddXHttpIPAddressAccessor()
                .AddXHttpIPAddressLocationAccessor()
                .Build();

            // comment to disable Interception
            services.AddTransient<ContactInterceptor>();

            services.AddHostedService<ContactContextInitializer>();
            services.AddRouting(options => options.ConstraintMap.Add("string", typeof(StringConstraintMap)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<IDataContext>();
                ((DbContext)context).Database.Migrate();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
