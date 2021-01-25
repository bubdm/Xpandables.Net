
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xpandables.Net.Interception
{
    /// <summary>
    /// Provides the implementation of the <see cref="IInvocation" /> interface.
    /// </summary>
    internal sealed class Invocation : IInvocation
    {
        public MethodInfo InvocationMethod { get; }
        public object InvocationInstance { get; }
        public IParameterCollection Arguments { get; }
        public Exception? Exception { get; private set; }
        public object? ReturnValue { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

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
            InvocationMethod = targetMethod ?? throw new ArgumentNullException(nameof(targetMethod));
            InvocationInstance = targetInstance ?? throw new ArgumentNullException(nameof(targetInstance));
            Arguments = new ParameterCollection(targetMethod, argsValue);
        }

        public IInvocation AddException(Exception? exception) => this.With(invocation => invocation.Exception = exception);
        public IInvocation AddReturnValue(object? returnValue) => this.With(invocation => invocation.ReturnValue = returnValue);
        public IInvocation AddElapsedTime(TimeSpan elapsedTime) => this.With(invocation => invocation.ElapsedTime = elapsedTime);
        public void Proceed()
        {
            var watch = Stopwatch.StartNew();
            watch.Start();

            try
            {
                ReturnValue = InvocationMethod.Invoke(
                                    InvocationInstance,
                                    Arguments.Select(arg => arg.Value).ToArray());

                if (ReturnValue is Task {Exception: { }} task)
                    Exception = task.Exception!.GetBaseException();
            }
            catch (Exception exception) when (exception is TargetException
                                          || exception is ArgumentNullException
                                          || exception is TargetInvocationException
                                          || exception is TargetParameterCountException
                                          || exception is MethodAccessException
                                          || exception is InvalidOperationException
                                          || exception is NotSupportedException)
            {
                Exception = exception;
            }
            finally
            {
                watch.Stop();
                ElapsedTime = watch.Elapsed;
            }
        }
    }
}
