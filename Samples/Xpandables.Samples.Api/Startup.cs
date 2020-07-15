
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Xpandables.Net5.DependencyInjection;
using Xpandables.Samples.Api.Configurations;
using Xpandables.Samples.Api.Middlewares;

namespace Xpandables.Samples.Api
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
            services.AddSingleton<IConfigureOptions<AuthenticationOptions>, XpandablesAuthenticationOptions>();
            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, XpandablesJwtBearerOptions>();

            services.AddXLocalizationResources<Application.Localization.LocalizationResourceProvider>();

            services.AddControllers();

            services.AddHttpContextAccessor();
            services.AddXHttpHeaderAccessor();
            services.AddXInstanceCreator();

            services.AddXServiceExport(Configuration);

            // database context sql access
            services.AddXDataBase();

            // dispatcher
            services.AddXDispatcher();

            // IP Geo-Location
            services.AddXHttpRestClientIPGeoLocationHandler();

            services
                .AddAuthentication()
                .AddJwtBearer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseXLocalizationBehavior();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseMiddleware<XpandablesExceptionHandlerMiddleware>();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
