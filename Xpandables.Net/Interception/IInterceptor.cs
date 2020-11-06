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

namespace Xpandables.Net.Interception
{
    /// <summary>
    /// Base interface for types and instances for interception.
    /// Interceptors implementing this interface are called for each invocation of the pipelines that they're included in.
    /// We advise the use of decorator instead of interceptor.
    /// </summary>
    public interface IInterceptor : ICanHandle<IInvocation>
    {
        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// This is used to optimize interception. If the behaviors won't actually do anything then the interception
        /// mechanism can be skipped completely.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// The default behavior returns <see langword="true"/>.
        /// </summary>
        /// <param name="argument">The argument to handle.</param>
        /// <returns><see langword="true"/> if it can handle the argument, otherwise <see langword="false"/></returns>
        public new bool CanHandle(IInvocation argument) => true;

        /// <summary>
        /// Method used to intercept the parameter method call.
        /// You have to call the <see cref="IInvocation.Proceed"/> to execute the intercepted method.
        /// The default implementation just call the <see cref="IInvocation.Proceed"/> method.
        /// </summary>
        /// <param name="invocation">The method argument to be called.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is null.</exception>
        public virtual void Intercept(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            invocation.Proceed();
        }
    }
}
