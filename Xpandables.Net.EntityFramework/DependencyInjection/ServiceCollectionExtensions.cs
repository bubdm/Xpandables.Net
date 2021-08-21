﻿
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Aggregates;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Provides method to register EFCore objects.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <typeparamref name="TDataContext"/> type class reference implementation derives from <see cref="Context"/> to the services with scoped life time.
    /// </summary>
    /// <typeparam name="TDataContext">The type of the data context that derives from <see cref="Context"/>.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <param name="optionsAction">An optional action to configure the Microsoft.EntityFrameworkCore.DbContextOptions for the context.</param>
    /// <param name="contextLifetime">The lifetime with which to register the context service in the container.</param>
    /// <param name="optionsLifetime">The lifetime with which to register the DbContextOptions service in the container.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXDataContext<TDataContext>(
        this IXpandableServiceBuilder services,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDataContext : Context
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddDbContext<TDataContext>(optionsAction, contextLifetime, optionsLifetime);
        return services;
    }

    /// <summary>
    /// Adds the <typeparamref name="TAggregateUnitOfWork"/> as <see cref="IAggregateUnitOfWork"/> to the services with scope life time.
    /// </summary>
    /// <typeparam name="TAggregateUnitOfWork">The type of aggregate unit of work.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXAggregateUnitOfWork<TAggregateUnitOfWork>(this IXpandableServiceBuilder services)
        where TAggregateUnitOfWork : class, IAggregateUnitOfWork
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Services.AddScoped<IAggregateUnitOfWork, TAggregateUnitOfWork>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="AggregateUnitOfWork{TContext}"/> using the <typeparamref name="TContext"/> as <see cref="IAggregateUnitOfWork"/> to the services with scope life time.
    /// </summary>
    /// <typeparam name="TContext">The type of the context for unit of work.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXAggregateUnitOfWorkContext<TContext>(this IXpandableServiceBuilder services)
        where TContext : Context
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        services.AddXUnitOfWorkContextFactory<UnitOfWorkContextFactory>();
        services.Services.AddScoped<IAggregateUnitOfWork, AggregateUnitOfWork<TContext>>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="UnitOfWork{TContext}"/> using the <typeparamref name="TContext"/> as <see cref="IUnitOfWork"/> to the services with scope life time.
    /// </summary>
    /// <typeparam name="TContext">The type of the context for unit of work.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public static IXpandableServiceBuilder AddXUnitOfWorkContext<TContext>(this IXpandableServiceBuilder services)
        where TContext : Context
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        services.AddXUnitOfWorkContextFactory<UnitOfWorkContextFactory>();
        services.Services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        return services;
    }

}
