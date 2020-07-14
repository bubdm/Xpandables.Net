
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xpandables.Net5.Expressions;

namespace Xpandables.Net5.Windows.Forms
{
    /// <summary>
    /// Provides with a mechanism to dynamically bind data in Windows Form.
    /// </summary>
    /// <typeparam name="TSource">The data source type to be used as data-source</typeparam>
    public sealed class DynamicDataBinding<TSource> : Disposable
        where TSource : class
    {
        private bool _isDisposed;
        private readonly System.Windows.Forms.BindingSource _bindingSource = new System.Windows.Forms.BindingSource();

        /// <summary>
        /// Initializes a new instance of <see cref="DynamicDataBinding{TSource}"/> with a data source.
        /// </summary>
        /// <param name="source">The data source instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public DynamicDataBinding(TSource source) => _bindingSource.DataSource = source ?? throw new ArgumentNullException(nameof(source));

        /// <summary>
        /// Gets the data source instance.
        /// </summary>
        public TSource Source => (TSource)_bindingSource.DataSource;

        /// <summary>
        /// Creates a binding between a property of a collection of controls and a property of a data source.
        /// </summary>
        /// <typeparam name="TControl">The control <typeparamref name="TControl"/>.</typeparam>
        /// <typeparam name="TControlProperty">The control property type.</typeparam>
        /// <typeparam name="TModelProperty">The data source property type.</typeparam>
        /// <param name="controls">Contains a collection of control instances.</param>
        /// <param name="controlPropertyAccessor">Contains the expression used to specify the control property.</param>
        /// <param name="sourcePropertyAccessor">Contains the expression used to specify the source property to bind to control property.</param>
        /// <param name="controlTransformAccessor">Contains the expression for applying a custom source property transformation.
        /// It's used to specify the binding format event. It can be null.</param>
        /// <param name="sourceUpdateMode">Contains the data source update mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="controls"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourcePropertyAccessor"/> is null.</exception>
        public void MultipleBinding<TControl, TControlProperty, TModelProperty>(
            IEnumerable<TControl> controls,
            Expression<Func<TControl, TControlProperty>> controlPropertyAccessor,
            Expression<Func<TSource, TModelProperty>> sourcePropertyAccessor,
            Func<TModelProperty, TControlProperty>? controlTransformAccessor = default,
            System.Windows.Forms.DataSourceUpdateMode sourceUpdateMode = System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged)
            where TControl : System.Windows.Forms.Control
        {
            if (controls?.Any() != true) throw new ArgumentNullException(nameof(controls));
            foreach (var control in controls)
                Binding(control, controlPropertyAccessor, sourcePropertyAccessor, controlTransformAccessor, sourceUpdateMode);
        }

        /// <summary>
        /// Creates a binding between a property of a control and a property of a data source.
        /// </summary>
        /// <typeparam name="TControl">The control <typeparamref name="TControl"/>.</typeparam>
        /// <typeparam name="TControlProperty">The control property type.</typeparam>
        /// <typeparam name="TModelProperty">The data source property type.</typeparam>
        /// <param name="control">Contains the instance of the control.</param>
        /// <param name="controlPropertyAccessor">Contains the expression used to specify the control property.</param>
        /// <param name="sourcePropertyAccessor">Contains the expression used to specify the source property to bind to control property.</param>
        /// <param name="controlTransformAccessor">Contains the expression for applying a custom source property transformation.
        /// It's used to specify the binding format event. It can be null.</param>
        /// <param name="sourceUpdateMode">Contains the data source update mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="control"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourcePropertyAccessor"/> is null.</exception>
        public void Binding<TControl, TControlProperty, TModelProperty>(
            TControl control,
            Expression<Func<TControl, TControlProperty>> controlPropertyAccessor,
            Expression<Func<TSource, TModelProperty>> sourcePropertyAccessor,
            Func<TModelProperty, TControlProperty>? controlTransformAccessor = default,
            System.Windows.Forms.DataSourceUpdateMode sourceUpdateMode = System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged)
            where TControl : System.Windows.Forms.Control
        {
            var propertyName = controlPropertyAccessor?.GetMemberName() ?? throw new ArgumentNullException(nameof(controlPropertyAccessor));
            var sourcePropetyName = sourcePropertyAccessor?.GetMemberName() ?? throw new ArgumentNullException(nameof(sourcePropertyAccessor));
            var binding = control?.DataBindings.Add(propertyName, _bindingSource, sourcePropetyName, true, sourceUpdateMode)
                ?? throw new ArgumentNullException(nameof(control));
            if (controlTransformAccessor is { })
                binding.Format += (sender, e) => e.Value = controlTransformAccessor((TModelProperty)e.Value);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        /// <remarks>
        /// <list type="bulle ted">
        /// <see cref="Dispose(bool)" /> executes in two distinct scenarios.
        /// <item>If <paramref name="disposing" /> equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources can be disposed.</item>
        /// <item>If <paramref name="disposing" /> equals <c>false</c>, the method has been called
        /// by the runtime from inside the finalizer and you should not reference other objects.
        /// Only unmanaged resources can be disposed.</item></list>
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
                _bindingSource?.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
