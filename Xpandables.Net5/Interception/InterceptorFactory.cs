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

namespace System.Design.Interception
{
    /// <summary>
    /// The interceptor class factory.
    /// </summary>
    public static class InterceptorFactory
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="TInterface"/> that will be intercepted with the specified interceptor.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="interceptor">An interceptor instance to apply.</param>
        /// <param name="instance">The real instance of the interface type.</param>
        /// <returns><typeparamref name="TInterface"/> proxy instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        public static TInterface CreateProxy<TInterface>(IInterceptor interceptor, TInterface instance)
            where TInterface : class
            => InterceptorProxy<TInterface>.Create(instance, interceptor);

        /// <summary>
        /// Creates an instance that will be intercepted with the specified interceptor.
        /// </summary>
        /// <param name="interfaceType">The type of the instance</param>
        /// <param name="interceptor">An interceptor instance to apply</param>
        /// <param name="instance">The real instance of the type</param>
        /// <returns><see cref="object"/> proxy instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> is null</exception>
        public static object CreateProxy(Type interfaceType, IInterceptor interceptor, object instance)
        {
            _ = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            _ = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
            _ = instance ?? throw new ArgumentNullException(nameof(instance));

            var proxyType = typeof(InterceptorProxy<>)
                .MakeGenericType(new Type[] { interfaceType })
                .GetMethod("Create", Reflection.BindingFlags.Public | Reflection.BindingFlags.Static)!;

            return proxyType.Invoke(null, new object[] { instance, interceptor })!;
        }
    }
}
