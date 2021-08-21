
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
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Collections;

namespace Xpandables.Net.Razors;

/// <summary>
/// Implementation of <see cref="IRazorModelViewCollection"/>.
/// </summary>
public sealed class RazorModelViewCollection : IRazorModelViewCollection
{
    private readonly IEnumerable<(Type ModelType, string Identifier)> _modelIdentifiers;

    /// <summary>
    /// Initializes a new instance of <see cref="RazorModelViewCollection"/>.
    /// </summary>
    /// <param name="applicationPartManager">The application part manager.</param>
    public RazorModelViewCollection(ApplicationPartManager applicationPartManager)
    {
        _modelIdentifiers = BuildModelViews(applicationPartManager);
    }

    private IEnumerable<(Type Model, string Identifier)> BuildModelViews(ApplicationPartManager applicationPartManager)
    {
        var feature = new ViewsFeature();
        applicationPartManager.PopulateFeature(feature);

        var views = feature.ViewDescriptors
            .Select(descr => GetModelType(descr.Item))
            .Where(item => item.HasModel)
            .Select(item => (item.Model, item.Identifier));

        foreach (var (model, identifier) in views)
        {
            if (model is { } && identifier is { })
                yield return (model, identifier);
        }
    }

    private (bool HasModel, string Identifier, Type? Model) GetModelType(RazorCompiledItem item)
    {
        var (hasModel, model) = GetModelType(item.Type);
        return (hasModel, item.Identifier, model);
    }

    private (bool hasModel, Type? Model) GetModelType(Type type)
    {
        if (type.BaseType is null || type == typeof(object))
            return (false, default);

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(RazorPage<>))
            {
                var genericArguments = type.GetGenericArguments();
                if (genericArguments.Length == 1)
                    return (true, genericArguments[0]);
            }
        }

        return GetModelType(type.BaseType);
    }

    /// <inheritdoc />
    public IEnumerator<(Type ModelType, string Identifier)> GetEnumerator() => _modelIdentifiers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
