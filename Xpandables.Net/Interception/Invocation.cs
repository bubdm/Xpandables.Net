
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
using System.Reflection;

namespace Xpandables.Net.Interception
{
    /// <summary>
    /// Provides the implementation of the <see cref="IInvocation" /> interface.
    /// </summary>
    internal sealed class Invocation : IInvocation
    {
        private readonly MethodInfo _invocationMethod;
        private readonly object _invocationInstance;
        private IParameterCollection _arguments;

        MethodInfo IInvocation.InvocationMethod => _invocationMethod;
        object IInvocation.InvocationInstance => _invocationInstance;
        IParameterCollection IInvocation.Arguments { get => _arguments; set => _arguments = value; }

        /// <summary>
        /// Initializes a new instance of <see cref="Invocation"/> with the arguments needed for invocation.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <param name="targetInstance">The target instance being called.</param>
        /// <param name="argsValue">Arguments for the method, if necessary.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetMethod"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetInstance"/> is null.</exception>
        internal Invocation(MethodInfo targetMethod, object targetInstance, params object?[]? argsValue)
        {
            _invocationMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            _invocationInstance = targetInstance ?? throw new ArgumentNullException(nameof(targetInstance));
            _arguments = new ParameterCollection(targetMethod, argsValue);
        }

        Exception? IInvocation.Exception { get; set; }
        object? IInvocation.ReturnValue { get; set; }
        TimeSpan IInvocation.ElapsedTime { get; set; }
    }
}
