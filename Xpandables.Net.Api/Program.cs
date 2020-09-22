
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
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Api.Middlewares;

namespace Xpandables.Net.Api
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
                .ConfigureAppConfiguration((buildContext, config) =>
                {
                    var env = buildContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", false, true)
                             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", false, true)
                             .AddEnvironmentVariables();

                    if (env.IsDevelopment())
                        config.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
                })
                 .ConfigureServices((_, services) =>
                 {
                     services.AddTransient<UserExceptionHandlerMiddleware>();
                     services.AddTransient<UserExceptionHandlerFilterAttribute>();
                     //services.AddScoped<LoggerHandlerMiddleware>();
                 })
                 .ConfigureLogging(loggerBuilder =>
                 {
                     loggerBuilder.ClearProviders();
                     loggerBuilder.AddConsole();
                     loggerBuilder.AddDebug();
                     loggerBuilder.AddEventLog();
                     loggerBuilder.AddEventSourceLogger();
                     loggerBuilder.AddTraceSource("Information, ActivityTracing");
                 })
               .UseKestrel(options =>
               {
                   options.AddServerHeader = false;
                   options.ListenLocalhost(5000);
               })
               .UseKestrel(options => options.AddServerHeader = false)
               .UseIIS()
               .UseIISIntegration()
               .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                    .UseStartup<Startup>();
            });
    }
}
