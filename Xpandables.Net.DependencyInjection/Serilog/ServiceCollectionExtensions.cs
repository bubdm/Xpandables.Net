
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

using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Configuration;

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;
using Xpandables.Net.Serilog;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register <see cref="Serilog"/> logger.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="loggerType">The logger type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="loggerType"/> is null.</exception>
        public static IServiceCollection AddXLoggingDecorator(this IServiceCollection services, Type loggerType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = loggerType ?? throw new ArgumentNullException(nameof(loggerType));

            if (!typeof(ILoggerEngine).IsAssignableFrom(loggerType))
                throw new ArgumentException($"{nameof(loggerType)} must implement {nameof(ILoggerEngine)}.");

            services.AddScoped(typeof(ILoggerEngine), loggerType);
            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandLoggingDecorator<>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryLoggingDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services
        /// </summary>
        /// <typeparam name="TLogger">The type of the logger.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXLoggingDecorator<TLogger>(this IServiceCollection services)
            where TLogger : class, ILogger
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddXLoggingDecorator(typeof(TLogger));
        }

        /// <summary>
        /// Adds the logger using the default <see cref="DefaultLogEntity"/> event to the services with scoped life time.
        /// The <see cref="DefaultLogEntity"/> must be registered in the data context and the context
        /// must implement the <see cref="IDataLogContext"/> interface.
        /// </summary>
        /// <param name="services">The current services collection.</param>
        /// <exception cref="InvalidOperationException">Registration failed.</exception>
        public static IServiceCollection AddXSerilog(this IServiceCollection services)
            => services.DoAddCustomSerilog<DefaultLogEntity>();

        /// <summary>
        /// Adds the logger using the specified type of log event to the services with scope life time.
        /// The <typeparamref name="TLogEntity"/> must be registered in the data context and the context
        /// must implement the <see cref="IDataLogContext"/> interface.
        /// </summary>
        /// <typeparam name="TLogEntity">The type of the log entity.</typeparam>
        /// <param name="services">The current services collection.</param>
        /// <exception cref="InvalidOperationException">Registration failed.</exception>
        public static IServiceCollection AddXSerilog<TLogEntity>(this IServiceCollection services)
            where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
            => services.DoAddCustomSerilog<TLogEntity>();

        /// <summary>
        /// Adds the <see cref="IDataLogContext"/> using the default <see cref="DefaultLogEntity"/> accessor to the services with scoped life time.
        /// Also registers the <see cref="Serilog"/> instance.
        /// </summary>
        /// <typeparam name="TDataLogContextProvider">The type of data context accessor
        /// that implements <see cref="IDataLogContextProvider{TLogEntity}"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataLogContext<TDataLogContextProvider>(this IServiceCollection services)
            where TDataLogContextProvider : class, IDataLogContextProvider<DefaultLogEntity>
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddXDataLogContext<TDataLogContextProvider, DefaultLogEntity>();
        }

        /// <summary>
        /// Adds the <see cref="IDataLogContext"/> accessor to the services with scoped life time.
        /// Also registers the <see cref="Serilog"/> instance.
        /// </summary>
        /// <typeparam name="TDataLogContextProvider">The type of data context accessor
        /// that implements <see cref="IDataLogContextProvider{TLogEntity}"/>.</typeparam>
        /// <typeparam name="TLogEntity">The type of the log entity.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataLogContext<TDataLogContextProvider, TLogEntity>(this IServiceCollection services)
            where TDataLogContextProvider : class, IDataLogContextProvider<TLogEntity>
            where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDataLogContextProvider<TLogEntity>, TDataLogContextProvider>();
            services.AddScoped(serviceProvider =>
            {
                var dataLogContextProvider = serviceProvider.GetRequiredService<IDataLogContextProvider<TLogEntity>>();
                var dataLogContext = dataLogContextProvider.GetDataLogContext();

                return dataLogContext;
            });
            services.AddXSerilog<TLogEntity>();
            return services;
        }

        private static IServiceCollection DoAddCustomSerilog<TLogEntity>(this IServiceCollection services)
                where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddScoped<LogEntitySink<TLogEntity>>();
            services.AddScoped(provider => CreateLoggerConfiguration<TLogEntity>(provider));

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services;
        }

        [DebuggerStepThrough]
        private static ILogger CreateLoggerConfiguration<TLogEntity>(IServiceProvider provider)
            where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
            => new LoggerConfiguration()
                .WriteTo
                .EntityFrameworkSink<TLogEntity>(provider)
                .CreateLogger();

        [DebuggerStepThrough]
        private static LoggerConfiguration EntityFrameworkSink<TLogEntity>(
            this LoggerSinkConfiguration sinkConfiguration,
            IServiceProvider provider)
            where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
            => sinkConfiguration.Sink(provider.GetRequiredService<LogEntitySink<TLogEntity>>());
    }
}
