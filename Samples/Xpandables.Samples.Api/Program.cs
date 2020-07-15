using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Reflection;

using Xpandables.Samples.Api.Middlewares;

namespace Xpandables.Samples.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration((builder, config) =>
                        {
                            var env = builder.HostingEnvironment;
                            config.SetBasePath(env.ContentRootPath);
                            config
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", false, true)
                                .AddEnvironmentVariables();

                            if (env.IsDevelopment())
                                config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
                        })
                        .ConfigureServices((_, services) =>
                        {
                            services.AddTransient<XpandablesExceptionHandlerMiddleware>();
                        })
                    .UseStartup<Startup>();
                });
    }
}
