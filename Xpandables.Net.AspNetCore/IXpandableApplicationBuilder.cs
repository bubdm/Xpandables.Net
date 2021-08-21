
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
using Microsoft.AspNetCore.Builder;

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// Allows integration of Xpandables.Net to the services collection.
/// </summary>
public interface IXpandableApplicationBuilder
{
    internal IApplicationBuilder Builder { get; }

    /// <summary>
    /// Returns the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <returns>The current instance of <see cref="IApplicationBuilder"/>.</returns>
    public IApplicationBuilder Build() => Builder;
}

/// <summary>
/// The <see cref="IXpandableServiceBuilder"/> implementation
/// </summary>
internal sealed class XpandableApplicationBuilder : IXpandableApplicationBuilder
{
    private readonly IApplicationBuilder _builder;
    IApplicationBuilder IXpandableApplicationBuilder.Builder => _builder;

    /// <summary>
    /// Constructs a new instance of <see cref="XpandableApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The service collection</param>
    /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
    public XpandableApplicationBuilder(IApplicationBuilder builder) => _builder = builder ?? throw new ArgumentNullException(nameof(builder));
}
