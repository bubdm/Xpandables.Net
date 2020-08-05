
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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xpandables.NetCore.Semantic
{
    /// <summary>
    /// Component base for semantic components
    /// </summary>
    public abstract class ComponentBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        /// <summary>
        /// An element type to render as.
        /// </summary>
        [Parameter]
        public string? As { get; set; }

        /// <summary>
        /// An element component to render as.
        /// </summary>
        [Parameter]
        public Type? AsComponent { get; set; }

        /// <summary>
        /// Child content
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Other input attributes
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
#pragma warning disable CA2227 // Collection properties should be read only
        public IDictionary<string, object>? InputAttributes { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Html Tag
        /// </summary>
        protected string ElementTag { get; set; }

        /// <summary>
        /// CSS classes
        /// </summary>
        protected string ElementClass { get; set; }

        /// <summary>
        /// All component attributes
        /// </summary>
        protected IDictionary<string, object> ElementAttributes { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ComponentBase"/>.
        /// </summary>
        protected ComponentBase()
        {
            ElementTag = "div";
            ElementClass = "";
            ElementAttributes = new Dictionary<string, object>();
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            if (!ElementAttributes.ContainsKey("Class"))
                ElementAttributes.Add("Class", "");

            BuildComponent();
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (AsComponent is default(Type) && InputAttributes != null)
            {
                foreach (var attribute in InputAttributes)
                {
                    // TODO Remove
                    Console.WriteLine(attribute.Key);
                }

                foreach (var attribute in InputAttributes)
                {
                    ThrowForUnknownIncomingParameterName(attribute.Key);
                }
            }

            BuildComponent();
        }

        /// <summary>
        /// Build the render tree.
        /// </summary>
        /// <param name="builder">The builder to be used.</param>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (AsComponent != null)
            {
                builder.OpenComponent(0, AsComponent);
                builder.AddMultipleAttributes(1, ElementAttributes);
                builder.AddMultipleAttributes(2, InputAttributes);
                builder.CloseComponent();
            }
            else
            {
                builder.OpenElement(0, string.IsNullOrWhiteSpace(As) ? ElementTag : As);
                builder.AddMultipleAttributes(1, ElementAttributes);
                builder.AddContent(2, ChildContent);
                builder.CloseElement();
            }
        }

        /// <summary>
		/// Configure the semantic component to be rendered
		/// </summary>
		protected abstract void ConfigureComponent();

        private void BuildComponent()
        {
            ConfigureComponent();
            if (InputAttributes?.ContainsKey("Class") == true)
            {
                ElementClass = $"{ElementClass} {InputAttributes["Class"]}";
                InputAttributes.Remove("Class");
            }

            if (AsComponent != null && !ElementAttributes.ContainsKey(nameof(ChildContent)) && ChildContent != null)
            {
                ElementAttributes.Add(nameof(ChildContent), ChildContent);
            }

            ElementAttributes["Class"] = ElementClass;
        }

        private void ThrowForUnknownIncomingParameterName(string parameterName)
        {
            Type componentType = GetType();
            var property = componentType.GetProperty(parameterName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (property != null)
            {
                if (!property.IsDefined(typeof(ParameterAttribute)) && !property.IsDefined(typeof(CascadingParameterAttribute)))
                {
                    throw new InvalidOperationException("Object of type '" + componentType.FullName + "' has a property matching the name '" + parameterName + "', but it does not have [ParameterAttribute] or [CascadingParameterAttribute] applied.");
                }

                throw new InvalidOperationException("No writer was cached for the property '" + property.Name + "' on type '" + componentType.FullName + "'.");
            }

            throw new InvalidOperationException("Object of type '" + componentType.FullName + "' does not have a property matching the name '" + parameterName + "'.");
        }
    }
}
