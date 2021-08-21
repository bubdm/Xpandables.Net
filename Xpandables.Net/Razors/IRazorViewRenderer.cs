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

namespace Xpandables.Net.Razors;

/// <summary>
/// Provides with methods to render a razor view to string.
/// </summary>
public interface IRazorViewRenderer
{
    /// <summary>
    /// Returns a string of a rendered view with the specified model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="viewName">The path for the view.</param>
    /// <param name="model">The model instance.</param>
    /// <returns>A string that represents the rendered view.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="viewName"/> is null.</exception>
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);

    /// <summary>
    /// Returns a string of a rendered view that matches the specified model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="model">The model instance.</param>
    /// <returns>A string that represents the rendered view.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="model"/> is null.</exception>
    Task<string> RenderFromModelToStringAsync<TModel>(TModel model);
}
