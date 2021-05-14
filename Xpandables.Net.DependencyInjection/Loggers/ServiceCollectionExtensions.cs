
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
using Microsoft.Extensions.DependencyInjection;

using System;

using Xpandables.Net.Commands;
using Xpandables.Net.Decorators;
using Xpandables.Net.Decorators.Logging;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the operation result logging type to the services.
        /// </summary>
        /// <typeparam name="TOperationResultLogger">The operation result logger type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultLogger<TOperationResultLogger>(this IXpandableServiceBuilder services)
            where TOperationResultLogger : class, IOperationResultLogger
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Services.AddScoped<IOperationResultLogger, TOperationResultLogger>();
            return services;
        }

        /// <summary>
        /// Adds operation result logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services.
        /// You need to register your implementation of <see cref="IOperationResultLogger"/> separately.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultLoggerDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));
            services.Services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));
            services.Services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryLoggingDecorator<,>));
            services.Services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryLoggingDecorator<,>));

            return services;
        }

        /// <summary>
        /// Adds operation result logging behavior to commands and queries that are decorated with the <see cref="ILoggingDecorator"/> to the services.
        /// </summary>
        /// <typeparam name="TOperationResultLogger">The operation result logger type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultLoggerDecorator<TOperationResultLogger>(this IXpandableServiceBuilder services)
            where TOperationResultLogger : class, IOperationResultLogger
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXOperationResultLogger<TOperationResultLogger>();

            services.Services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandLoggingDecorator<>));
            services.Services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));
            services.Services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryLoggingDecorator<,>));
            services.Services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryLoggingDecorator<,>));

            return services;
        }
    }
}
