
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
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Xpandables.Net.Api.Configurations;
using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Api.Middlewares;
using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Services;
using Xpandables.Net.Api.Services.Implementations;
using Xpandables.Net.Api.Settings;
using Xpandables.Net.Api.Storage;
using Xpandables.Net.Commands;
using Xpandables.Net.Correlation;
using Xpandables.Net.DependencyInjection;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Queries;
using Xpandables.NetCore.Startup;

namespace Xpandables.Net.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.XConfigureOptions<JwtSettings>(Configuration);
            services.XConfigureOptions<DataContextSettings>(Configuration);

            services.AddSingleton<IConfigureOptions<AuthenticationOptions>, ApiAuthenticationOptions>();
            services.AddSingleton<IConfigureOptions<MvcOptions>, ApiMvcOptions>();
            services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, ApiNewtonsoftJsonOptions>();
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ApiJwtBearerOptions>();

            services.AddMemoryCache();

            services
                .AddControllers()
                .AddNewtonsoftJson();

            services.AddHttpContextAccessor();
            services.AddXInstanceCreator();
            services.AddXStringGeneratorCryptography();
            services.AddXHttpTokenAccessor();
            services.AddXHttpHeaderAccessor();

            services.AddScoped<AsyncCorrelationContext>();
            services.AddScoped<IAsyncCorrelationContext>(provider => provider.GetRequiredService<AsyncCorrelationContext>());
            services.AddXCorrelationContext();

            services.AddXHttpTokenEngine<TokenService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();

            services.AddXDataContext<UserContextProvider>(options => options.UseSeederDecorator<UserContextSeeder, UserContext>());

            services.AddXQueryHandlerWrapper();
            services.AddXIdentityDataProvider();

            services.AddScoped<IAsyncQueryHandler<RequestAuthenToken, AuthenToken>, RequestAuthenTokenHandler>();
            services.AddScoped<IAsyncCommandHandler<EditUser>, EditUserHandler>();
            services.AddScoped<IAsyncQueryHandler<EventLogList, Log>, EventLogListHandler>();

            services.AddXIdentityDecorator();
            services.AddXPersistenceDecorator();

            services.AddXDispatcher();
            services.AddAuthentication()
                    .AddJwtBearer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseMiddleware<UserExceptionHandlerMiddleware>();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
