﻿
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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Xpandables.Net.Interception
{
    /// <summary>
    /// The base implementation for interceptor.
    /// This implementation uses the <see cref="DispatchProxy" /> process to apply customer behaviors to a method.
    /// </summary>
    /// <typeparam name="TInterface">Type of interface.</typeparam>
    public class InterceptorProxy<TInterface> : DispatchProxy
        where TInterface : class
    {
        private static readonly MethodBase MethodBaseType = typeof(object).GetMethod("GetType")!;
        private TInterface _realInstance;
        private IInterceptor _interceptor;

        /// <summary>
        /// Returns a new instance of <typeparamref name="TInterface"/> wrapped by a proxy.
        /// </summary>
        /// <param name="instance">the instance to be wrapped.</param>
        /// <param name="interceptor">The instance of the interceptor.</param>
        /// <returns>An instance that has been wrapped by a proxy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        public static TInterface Create(TInterface instance, IInterceptor interceptor)
        {
            object proxy = Create<TInterface, InterceptorProxy<TInterface>>();
            ((InterceptorProxy<TInterface>)proxy).SetParameters(instance, interceptor);
            return (TInterface)proxy;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="InterceptorProxy{TInstance}"/> with default values.
        /// </summary>
        public InterceptorProxy()
        {
            _realInstance = default!;
            _interceptor = default!;
        }

        /// <summary>
        /// Initializes the decorated instance and the interceptor with the provided arguments.
        /// </summary>
        /// <param name="instance">The instance to be intercepted.</param>
        /// <param name="interceptor">The instance of interceptor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is null.</exception>
        private void SetParameters(TInterface instance, IInterceptor interceptor)
        {
            _realInstance = instance ?? throw new ArgumentNullException(nameof(instance));
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <summary>
        /// Executes the method specified in the <paramref name="targetMethod" />.
        /// Applies the interceptor behavior to the called method.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="args">The expected arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="targetMethod" /> is null.</exception>
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            _ = targetMethod ?? throw new ArgumentNullException($"The parameter {nameof(targetMethod)} is missing.");

            return ReferenceEquals(targetMethod, MethodBaseType)
                ? Bypass(targetMethod, args)
                : DoInvoke(targetMethod, args);
        }

        private object? DoInvoke(MethodInfo method, params object?[]? args)
        {
            IInvocation invocation = new Invocation(method, _realInstance, args);

            if (_interceptor.CanHandle(invocation))
            {
                try
                {
                    _interceptor.Intercept(invocation);
                }
                catch (Exception exception)
                {
                    invocation.AddException(
                        new InvalidOperationException(
                            $"The interceptor {_interceptor.GetType().Name} throws an exception.",
                            exception));
                }
            }
            else
            {
                invocation.Proceed();
            }

            if (invocation.Exception is { } ex)
                ExceptionDispatchInfo.Capture(ex).Throw();

            return invocation.ReturnValue;
        }

        /// <summary>
        /// Bypass the interceptor application because the method is a system method (GetType).
        /// </summary>
        /// <param name="targetMethod">Contains all information about the method being executed</param>
        /// <param name="args">Arguments to be used.</param>
        /// <returns><see cref="object"/> instance</returns>
        [DebuggerStepThrough]
        private object? Bypass(MethodInfo targetMethod, object?[]? args) => targetMethod.Invoke(_realInstance, args);
    }
}
