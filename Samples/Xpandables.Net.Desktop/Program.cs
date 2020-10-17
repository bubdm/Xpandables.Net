
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
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Windows.Forms;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Xpandables.Net.DependencyInjection;
using Xpandables.Net.Desktop.Models;
using Xpandables.Net.Desktop.Settings;
using Xpandables.Net.HttpRestClient;
using Xpandables.Samples.Desktop;
using Xpandables.Samples.Desktop.Views;

namespace Xpandables.Net.Desktop
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

                    services.AddTransient<MainFormViewModel>();
                    services.AddTransient<LoginFormViewModel>();
                    services.AddTransient<MainForm>();
                    services.AddTransient<LoginForm>();

                    services.AddXHttpTokenDelegateAccessor();
                    services.AddXHttpRestClientEngine();
                    services.AddHttpClient<IHttpRestClientHandler, HttpRestClientHandler>("ApiClient", (serviceProvider, httpClient) =>
                    {
                        var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
                        httpClient.BaseAddress = new Uri($"{apiSettings.Url}/");
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    })
                    .ConfigureXPrimaryAuthorizationTokenHandler();
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
