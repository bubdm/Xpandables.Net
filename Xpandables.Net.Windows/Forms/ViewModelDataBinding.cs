
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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

using Xpandables.Net.Expressions;

namespace Xpandables.Net.Forms
{
    /// <summary>
    /// Provides with a mechanism to dynamically bind data in Windows Form.
    /// </summary>
    /// <typeparam name="TViewModel">The view model source type to be used for binding.</typeparam>
    public abstract class ViewModelDataBinding<TViewModel> : Form
        where TViewModel : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ViewModelDataBinding{TViewModel}"/> class that initializes the data to its default value and the binding source.
        /// </summary>
        protected ViewModelDataBinding(TViewModel viewModel)
        {
            ViewModel = viewModel;
            BindingSource = new BindingSource() { DataSource = ViewModel };
        }

        /// <summary>
        /// Creates a binding between a property of a collection of controls and a property of a data source.
        /// </summary>
        /// <typeparam name="TControl">The control <typeparamref name="TControl"/>.</typeparam>
        /// <typeparam name="TControlProperty">The control property type.</typeparam>
        /// <typeparam name="TDataProperty">The data source property type.</typeparam>
        /// <param name="controls">Contains a collection of control instances.</param>
        /// <param name="controlPropertyAccessor">Contains the expression used to specify the control property.</param>
        /// <param name="dataPropertyAccessor">Contains the expression used to specify the source property to bind to control property.</param>
        /// <param name="controlTransformAccessor">Contains the expression for applying a custom source property transformation.
        /// It's used to specify the binding format event. It can be null.</param>
        /// <param name="dataUpdateMode">Contains the data source update mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="controls"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPropertyAccessor"/> is null.</exception>
        public void MultipleBinding<TControl, TControlProperty, TDataProperty>(
            IEnumerable<TControl> controls,
            Expression<Func<TControl, TControlProperty>> controlPropertyAccessor,
            Expression<Func<TViewModel, TDataProperty>> dataPropertyAccessor,
            Func<TDataProperty, TControlProperty>? controlTransformAccessor = default,
            DataSourceUpdateMode dataUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
            where TControl : Control
        {
            if (controls?.Any() != true) throw new ArgumentNullException(nameof(controls));
            foreach (var control in controls)
                Binding(control, controlPropertyAccessor, dataPropertyAccessor, controlTransformAccessor, dataUpdateMode);
        }

        /// <summary>
        /// Creates a binding between a property of a control and a property of a data source.
        /// </summary>
        /// <typeparam name="TControl">The control <typeparamref name="TControl"/>.</typeparam>
        /// <typeparam name="TControlProperty">The control property type.</typeparam>
        /// <typeparam name="TDataProperty">The data source property type.</typeparam>
        /// <param name="control">Contains the instance of the control.</param>
        /// <param name="controlPropertyAccessor">Contains the expression used to specify the control property.</param>
        /// <param name="dataPropertyAccessor">Contains the expression used to specify the source property to bind to control property.</param>
        /// <param name="controlTransformAccessor">Contains the expression for applying a custom source property transformation.
        /// It's used to specify the binding format event. It can be null.</param>
        /// <param name="dataUpdateMode">Contains the data source update mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="control"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="controlPropertyAccessor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPropertyAccessor"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="controlPropertyAccessor"/> returns null value.</exception>
        /// <exception cref="ArgumentException">The <paramref name="dataPropertyAccessor"/> returns null value.</exception>
        public void Binding<TControl, TControlProperty, TDataProperty>(
            TControl control,
            Expression<Func<TControl, TControlProperty>> controlPropertyAccessor,
            Expression<Func<TViewModel, TDataProperty>> dataPropertyAccessor,
            Func<TDataProperty, TControlProperty>? controlTransformAccessor = default,
            DataSourceUpdateMode dataUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
            where TControl : class, IBindableComponent
        {
            _ = control ?? throw new ArgumentNullException(nameof(control));
            _ = controlPropertyAccessor ?? throw new ArgumentNullException(nameof(controlPropertyAccessor));
            _ = dataPropertyAccessor ?? throw new ArgumentNullException(nameof(dataPropertyAccessor));

            var propertyName = controlPropertyAccessor.GetMemberName() ?? throw new ArgumentException($"{nameof(controlPropertyAccessor)} returns null");
            var sourcePropetyName = dataPropertyAccessor.GetMemberName() ?? throw new ArgumentException($"{nameof(dataPropertyAccessor)} returns null");

            var binding = control.DataBindings.Add(propertyName, BindingSource, sourcePropetyName, true, dataUpdateMode);
            if (controlTransformAccessor is { })
                binding.Format += (sender, e) => e.Value = controlTransformAccessor((TDataProperty)e.Value);
        }

        /// <summary>
        /// Gets the view model source current instance.
        /// </summary>
        protected TViewModel ViewModel { get; }

        /// <summary>
        /// Gets the binding source instance.
        /// </summary>
        protected BindingSource BindingSource { get; }

        private bool _isDisposed;
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
                BindingSource?.Dispose();

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
