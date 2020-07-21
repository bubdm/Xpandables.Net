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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;

using Xpandables.Net.Helpers;

namespace Xpandables.Net.Creators
{
    /// <summary>
    /// Provides with methods to create instance of specific type at runtime using delegate and cache.
    /// </summary>
    public interface IInstanceCreator
    {
        /// <summary>
        /// Contains the instance cache.
        /// </summary>
        ConcurrentDictionary<string, Delegate> Cache { get; }

        /// <summary>
        /// Define an action that will be called in case of handled exception during a create method execution.
        /// </summary>
        Action<ExceptionDispatchInfo>? OnException { get; }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> or null if exception.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <param name="type">The type to be created.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        [return: MaybeNull]
        public object Create(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<object>>(type, Array.Empty<Type>());
                return lambdaConstructor.Invoke();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> or null if exception.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param">The parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="param"/> is null.</exception>
        [return: MaybeNull]
        public object Create<TParam>(Type type, TParam param)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = param ?? throw new ArgumentNullException(nameof(param));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam, object>>(type, new[] { typeof(TParam) });
                return lambdaConstructor.Invoke(param);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> or null if exception.
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
        [return: MaybeNull]
        public object Create<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = param1 ?? throw new ArgumentNullException(nameof(param1));
            _ = param2 ?? throw new ArgumentNullException(nameof(param2));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam1, TParam2, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2) });

                return lambdaConstructor.Invoke(param1, param2);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> or null if exception.
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
        [return: MaybeNull]
        public object Create<TParam1, TParam2, TParam3>(Type type, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = param1 ?? throw new ArgumentNullException(nameof(param1));
            _ = param2 ?? throw new ArgumentNullException(nameof(param2));
            _ = param3 ?? throw new ArgumentNullException(nameof(param3));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam1, TParam2, TParam3, object>>(
                    type,
                    new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });

                return lambdaConstructor.Invoke(param1, param2, param3);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }


        /// <summary>
        /// Clear the constructor cache.
        /// </summary>
        public void ClearCache() => Cache.Clear();

        private TDelegate GetConstructorDelegate<TDelegate>(Type type, params Type[] parameterTypes)
                 where TDelegate : Delegate
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var key = KeyBuilder(type, parameterTypes);
            return (TDelegate)Cache.GetOrAdd(key, _ => ConstructDelegate(type, parameterTypes));

            static TDelegate ConstructDelegate(Type t, params Type[] types)
            {
                if (t.TryGetConstructorDelegate<TDelegate>(out var constructorDelegate, out var exception, types))
                    return constructorDelegate!;

                throw exception;
            }
        }

        // Build a key for a type
        private static string KeyBuilder(Type type, params Type[] parameterTypes)
        {
            var key = type.FullName!;
            if (parameterTypes.Length > 0) key += string.Concat(parameterTypes.Select(t => t.Name));
            if (type.IsGenericType) key += "'1";

            return key;
        }
    }

    /// <summary>
    /// Default implementation for <see cref="IInstanceCreator"/>.
    /// You can customize the behavior providing your own implementing of <see cref="IInstanceCreator"/> interface.
    /// </summary>
    public sealed class InstanceCreator : IInstanceCreator
    {
        /// <summary>
        /// Define an action that will be called in case of handled exception during a create method execution.
        /// </summary>
        public Action<ExceptionDispatchInfo>? OnException { get; }

        /// <summary>
        /// Contains the instance cache.
        /// </summary>
        public ConcurrentDictionary<string, Delegate> Cache { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="InstanceCreator"/> with the string generator.
        /// </summary>
        public InstanceCreator() => (Cache, OnException) = (new ConcurrentDictionary<string, Delegate>(), default);
    }
}
