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
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Provides with methods to create instance of specific type at runtime.
    /// </summary>
    public interface IInstanceCreator
    {
        /// <summary>
        /// Define an event that will be raised in case of handled exception during a create method execution.
        /// </summary>
        event Action<ExceptionDispatchInfo> OnException;

        /// <summary>
        /// Returns an instance of the <paramref name="type"/>.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <param name="type">The type to be created.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        object? Create(Type type);

        /// <summary>
        /// Returns an instance of the <paramref name="type"/>.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param">The parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param"/> is null.</exception>
        object? Create<TParam>(Type type, TParam param);

        /// <summary>
        /// Returns an instance of the <paramref name="type"/>.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param1">The first parameter to pass to the constructor.</param>
        /// <param name="param2">The first parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param1"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param2"/> is null.</exception>
        object? Create<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2);

        /// <summary>
        /// Returns an instance of the <paramref name="type"/>.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam3">The type of the third parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param1">The first parameter to pass to the constructor.</param>
        /// <param name="param2">The first parameter to pass to the constructor.</param>
        /// <param name="param3">The first parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param1"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param2"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param3"/> is null.</exception>
        object? Create<TParam1, TParam2, TParam3>(Type type, TParam1 param1, TParam2 param2, TParam3 param3);

        /// <summary>
        /// Dynamically creates a class named <see cref="InstanceClass"/> + counter from the <see cref="IEnumerable{InstanceProperty}"/> dynamic properties.
        /// </summary>
        /// <param name="properties">The properties list to be used</param>
        /// <returns><see cref="Type"/> with the specified properties</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static Type CreateClass(IEnumerable<InstanceProperty> properties) => InstanceClassFactory.Instance.GetInstanceClass(properties);

        /// <summary>
        /// Dynamically creates a class named <see cref="InstanceClass"/> + counter from the list of dynamic properties.
        /// </summary>
        /// <param name="properties">The arguments to be used</param>
        /// <returns><see cref="Type"/> with the specified properties</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static Type CreateClass(params InstanceProperty[] properties) => InstanceClassFactory.Instance.GetInstanceClass(properties);

        /// <summary>
        /// Clear the constructor cache.
        /// </summary>
        void ClearCache();
    }
}
