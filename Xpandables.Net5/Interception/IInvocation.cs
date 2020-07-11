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

using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.Interception
{
    /// <summary>
    /// Provides the structure for an interception event.
    /// </summary>
    public interface IInvocation
    {
        /// <summary>
        /// Contains the invocation target method info.
        /// </summary>
        internal MethodInfo InvocationMethod { get; }

        /// <summary>
        /// Contains the invocation target instance.
        /// </summary>
        internal  object InvocationInstance { get; }

        /// <summary>
        /// Contains the arguments (position in signature, names and values) with which the method has been invoked.
        /// This argument is provided only for target element with parameters.
        /// </summary>
        IParameterCollection Arguments { get; internal set; }

        /// <summary>
        /// Gets the exception handled on executing a method.
        /// You can edit this value in order to return a custom exception or null.
        /// If you set this value to null, the process will resume normally and
        /// take care to provide a <see cref="ReturnValue"/> if necessary.
        /// </summary>
        Exception? Exception { get; internal set; }

        /// <summary>
        /// Gets the executed method return value, only provided for non-void method and when no exception handled.
        /// </summary>
        object? ReturnValue { get; internal set; }

        /// <summary>
        /// Get the elapsed time execution for the underlying method.
        /// </summary>
        TimeSpan ElapsedTime { get; internal set; }

        /// <summary>
        /// Gets the invocation method return type.
        /// </summary>
        public sealed Type ReturnType => InvocationMethod.ReturnType;

        /// <summary>
        /// Sets the exception value.
        /// If you set this value to null, the process will resume normally and
        /// take care to provide a <see cref="ReturnValue" /> if necessary.
        /// </summary>
        /// <param name="exception">The exception value.</param>
        /// <returns>The current instance with exception value.</returns>
        public sealed IInvocation AddException(Exception? exception) => this.With(invocation => invocation.Exception = exception);

        /// <summary>
        /// Sets the executed method return value, only for non-void method.
        /// Be aware to match the return value type.
        /// Otherwise it will throw an exception.
        /// </summary>
        /// <param name="returnValue">The return value to be used.</param>
        /// <returns>The current instance with return value.</returns>
        public sealed IInvocation AddReturnValue(object? returnValue) => this.With(invocation => invocation.ReturnValue = returnValue);

        /// <summary>
        /// Sets the executed method elapsed time.
        /// </summary>
        /// <param name="elapsedTime">The method elapsed.</param>
        /// <returns>The current instance with the new elapsed time.</returns>
        public sealed IInvocation AddElapsedTime(TimeSpan elapsedTime) => this.With(invocation => invocation.ElapsedTime = elapsedTime);

        /// <summary>
        /// Executes the underlying method.
        /// </summary>
        public sealed void Proceed()
        {
            var watch = Stopwatch.StartNew();
            watch.Start();

            try
            {
                ReturnValue = InvocationMethod.Invoke(
                                    InvocationInstance,
                                    Arguments.Select(arg => arg.Value).ToArray());

                if (ReturnValue is Task task && task.Exception != null)
                    Exception = task.Exception.GetBaseException();
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
