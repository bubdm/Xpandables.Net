using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Windows.Forms;

using Xpandables.Net5.DependencyInjection;
using Xpandables.Samples.Desktop.Settings;
using Xpandables.Samples.Desktop.Views;

namespace Xpandables.Samples.Desktop
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    builder
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions();
                    services.Configure<ApiSettings>(context.Configuration.GetSection(nameof(ApiSettings)));

                    services.AddScoped<MainForm>();
                    services.AddScoped<LoginForm>();
                    services.AddXHttpRestClientHandler();

                    services.AddHttpClient("ApiClient", (serviceProvider, httpClient) =>
                    {
                        var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                        httpClient.BaseAddress = new Uri($"{apiSettings.Url}/");
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();

            using var serviceScope = host.Services.CreateScope();
            try
            {
                var services = serviceScope.ServiceProvider;
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var mainForm = services.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception exception) when (exception is InvalidOperationException)
            {
                Trace.WriteLine(exception);
            }
        }
    }
}
