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
using Microsoft.Extensions.DependencyInjection;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Xpandables.Net.AspNetCore, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]
[assembly: InternalsVisibleTo("Xpandables.Net.EntityFramework, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]
[assembly: InternalsVisibleTo("Xpandables.Net.BlazorExtended, PublicKey=0024000004800000940000000602000000240000525341310004000001000100410b9f6b317bb83c59c2727a39ad3e0c3aff55cbfc6f1328e2a925ab2e85d44b1815b23cea3f22924ea4226a6b3318eb90d1f28234e0116be8b70c29a41849a93e1baa680deae7f56e8d75d352d6f3b8457746223adf8cc2085a2d1d8c3f7be439bc53f1a032cc696f75afa378e0e054f3eb325fb9a7898a31c612c21e9c3cb8")]

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Allows integration of Xpandables.Net to the services collection.
/// </summary>
public interface IXpandableServiceBuilder
{
    internal IServiceCollection Services { get; }

    /// <summary>
    /// Returns the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The current instance of <see cref="IServiceCollection"/>.</returns>
    public IServiceCollection Build() => Services;
}

/// <summary>
/// The <see cref="IXpandableServiceBuilder"/> implementation
/// </summary>
internal sealed class XpandableServiceBuilder : IXpandableServiceBuilder
{
    private readonly IServiceCollection _services;
    IServiceCollection IXpandableServiceBuilder.Services => _services;

    /// <summary>
    /// Constructs a new instance of <see cref="XpandableServiceBuilder"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
    public XpandableServiceBuilder(IServiceCollection services) => _services = services ?? throw new ArgumentNullException(nameof(services));
}
