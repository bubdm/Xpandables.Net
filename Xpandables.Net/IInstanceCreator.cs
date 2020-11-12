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
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with methods to create instance of specific type at runtime using delegate and cache.
    /// </summary>
    public interface IInstanceCreator
    {
        /// <summary>
        /// Contains the instance cache that contains a string key for a constructor type and the target delegate.
        /// it can be cleared to free memory.
        /// </summary>
        ConcurrentDictionary<string, Delegate> Cache { get; }

        /// <summary>
        /// Represents the action that will be called in case of handled exception during a create method execution.
        /// </summary>
        Action<ExceptionDispatchInfo>? OnException { get; set; }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> with a parameterless constructor or null if exception.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <param name="type">The type to be created.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        [return: MaybeNull]
        public virtual object Create(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<object>>(type, Array.Empty<Type>());
                return lambdaConstructor.Invoke();
            }
            catch (Exception exception)
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance (using cache) of the <paramref name="type"/> with a constructor that takes an argument of a type-specific or null if exception.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param">The parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        [return: MaybeNull]
        public virtual object Create<TParam>(Type type, TParam param)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam, object>>(type, new[] { typeof(TParam) });
                return lambdaConstructor.Invoke(param);
            }
            catch (Exception exception)
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance (using cache) of the <paramref name="type"/> with a constructor that takes two arguments of specific-type or null if exception.
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param1">The first parameter to pass to the constructor.</param>
        /// <param name="param2">The first parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        [return: MaybeNull]
        public virtual object Create<TParam1, TParam2>(Type type, TParam1 param1, TParam2 param2)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam1, TParam2, object>>(
                    type, new[] { typeof(TParam1), typeof(TParam2) });

                return lambdaConstructor.Invoke(param1, param2);
            }
            catch (Exception exception)
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Returns an instance (using cache) of the <paramref name="type"/> with a constructor that takes three arguments of specific-type or null if exception..
        /// In case of exception, the <see cref="OnException"/> will be raised.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter to pass to the constructor.</typeparam>
        /// <typeparam name="TParam3">The type of the third parameter to pass to the constructor.</typeparam>
        /// <param name="type">The type to be created.</param>
        /// <param name="param1">The first parameter to pass to the constructor.</param>
        /// <param name="param2">The first parameter to pass to the constructor.</param>
        /// <param name="param3">The first parameter to pass to the constructor.</param>
        /// <returns>An instance of the <paramref name="type"/> if OK or null.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        [return: MaybeNull]
        public virtual object Create<TParam1, TParam2, TParam3>(Type type, TParam1 param1, TParam2 param2, TParam3 param3)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            try
            {
                var lambdaConstructor = GetConstructorDelegate<Func<TParam1, TParam2, TParam3, object>>(
                    type, new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) });

                return lambdaConstructor.Invoke(param1, param2, param3);
            }
            catch (Exception exception)
            {
                OnException?.Invoke(ExceptionDispatchInfo.Capture(exception));
                return default;
            }
        }

        /// <summary>
        /// Clear the creator cache.
        /// </summary>
        public virtual void ClearCache() => Cache.Clear();

        internal TDelegate GetConstructorDelegate<TDelegate>(Type type, params Type[] parameterTypes)
            where TDelegate : Delegate
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            var key = KeyBuilder(type, parameterTypes);
            return (TDelegate)Cache.GetOrAdd(key, _ => ConstructDelegate(type, parameterTypes));

            static TDelegate ConstructDelegate(Type t, params Type[] types)
            {
                return t.TryGetConstructorDelegate<TDelegate>(out var constructorDelegate, out var exception, types)
                    ? constructorDelegate
                    : throw exception;
            }
        }

        // Build a key for a type
        internal const string key = "b14ca5898a4e4133bbce2ea2315a1916";
        internal static string KeyBuilder(Type type, params Type[] parameterTypes)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];

            using var encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);

            var plainText = type.FullName!;
            if (parameterTypes.Any()) plainText += string.Concat(parameterTypes.Select(t => t.Name));
            if (type.IsGenericType) plainText += "'1";

            streamWriter.Write(plainText);
            var encrypted = memoryStream.ToArray();

            return Convert.ToBase64String(encrypted);
        }
    }

    /// <summary>
    /// The implementation for <see cref="IInstanceCreator"/>.
    /// You can customize the behavior providing your own, implementing of <see cref="IInstanceCreator"/> interface.
    /// </summary>
    public sealed class InstanceCreator : IInstanceCreator
    {
        /// <summary>
        /// Define an action that will be called in case of handled exception during a create method execution.
        /// </summary>
        public Action<ExceptionDispatchInfo>? OnException { get; set; }

        /// <summary>
        /// Contains the instance cache that can be cleared to free memory.
        /// </summary>
        public ConcurrentDictionary<string, Delegate> Cache { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="InstanceCreator"/> settings the cache to its default value.
        /// </summary>
        public InstanceCreator() => (Cache, OnException) = (new ConcurrentDictionary<string, Delegate>(), default);
    }
}
