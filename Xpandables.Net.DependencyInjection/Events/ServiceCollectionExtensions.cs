using System;
using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Configuration;

using Xpandables.Net.Events;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register <see cref="Serilog"/> logger.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
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
