
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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

using Xpandables.Net.Localization;

namespace Xpandables.Net.Windows.Wpf
{
    /// <summary>
    /// WPF localization converter for display attribute that uses the <see cref="ILocalizationResourceProvider"/>  as resource.
    /// </summary>
    public sealed class DisplayAttributeLocalizationConverter : MarkupExtension, IValueConverter
    {
        private ILocalizationResourceProvider _localizationValidationResource = default!;
        private INotifyPropertyChanged? _dataContext;

        /// <summary>
        /// When implemented in a derived class,
        /// returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _localizationValidationResource = serviceProvider.GetService(typeof(ILocalizationResourceProvider)) as ILocalizationResourceProvider
                ?? throw new ArgumentException($"Service {nameof(ILocalizationResourceProvider)} is unavailable.");

            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider
                ?? throw new ArgumentException($"Service {nameof(IRootObjectProvider)} is unavailable.");

            var window = rootProvider.RootObject as FrameworkElement ?? throw new InvalidOperationException("WPF FrameworkElement expected.");
            _dataContext = window.DataContext as INotifyPropertyChanged;

            return this;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter?.ToString() is not string propertyName) return string.Empty;
            if (_dataContext is null) return propertyName;

            if (_dataContext.GetType().GetProperty(propertyName) is not { } propertyInfo) return propertyName;

            if (propertyInfo.GetCustomAttribute<DisplayAttribute>() is not { } displayAttribute) return propertyName;
            if (displayAttribute.Name is null) return propertyName;

            if (_localizationValidationResource.ValidationType is { })
                displayAttribute.ResourceType = _localizationValidationResource.ValidationType;

            return displayAttribute.GetName()!;
        }

        /// <summary>
        /// Converts a value. There's not convert back.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
