
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
using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Xpandables.Net.Api.Configurations;
using Xpandables.Net.Api.Middlewares;
using Xpandables.Net.Api.Services;
using Xpandables.Net.Api.Services.Implementations;
using Xpandables.Net.Api.Settings;
using Xpandables.Net.Api.Storage;
using Xpandables.Net.DependencyInjection;

namespace Xpandables.Net.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.XConfigureOptions<JwtSettings>(Configuration);

            services.AddSingleton<IConfigureOptions<AuthenticationOptions>, ApiAuthenticationOptions>();
            services.AddSingleton<IConfigureOptions<MvcOptions>, ApiMvcOptions>();
            services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, ApiNewtonsoftJsonOptions>();
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ApiJwtBearerOptions>();

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("string", typeof(StringConstraintMap));
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddMemoryCache();

            services
                .AddControllers()
                .AddNewtonsoftJson();

            services.AddHttpContextAccessor();
            services.AddXStringGeneratorCryptography();
            services.AddXHttpHeaderAccessor();
            services.AddXHttpTokenClaimProvider<HttpTokenClaimProvider>();

            services.AddXHttpTokenEngine<TokenService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();

            services.AddDbContext<UserContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default"))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .EnableServiceProviderCaching());

            services.AddXDataContext(provider => provider.GetRequiredService<UserContext>());
            services.AddHostedService<UserContextInitializer>();

            services.AddXCommandQueriesHandlers(new[] { Assembly.GetExecutingAssembly() }, options =>
            {
                options.UsePersistenceDecorator();
                options.UseValidatorDecorator();
            });

            services.AddXDispatcher();
            services.AddAuthentication()
                    .AddJwtBearer();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Xpandables.Net.Api", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Xpandables.Net.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseMiddleware<ApiExceptionHandlerMiddleware>();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
