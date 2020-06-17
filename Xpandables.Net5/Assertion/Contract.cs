
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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Allows an application author to check arguments validation.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class Contract<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Contract{T}"/> class with the value to be checked, the
        /// predicate to be applied on this value and the exception message to be returned if the value don't match the predicate.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="predicate">The predicate to be applied.</param>
        /// <param name="parameterName">The default exception message.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public Contract(
            T value,
            Predicate<T> predicate,
            string parameterName)
        {
            Value = value;
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        [NotNull]
        public string ParameterName { get; }

        /// <summary>
        /// Gets the value to check.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the criteria to be applied.
        /// </summary>
        [NotNull]
        public Predicate<T> Predicate { get; }

        /// <summary>
        /// Determine whether or not the value matches the criteria.
        /// </summary>
        public bool IsValid => Predicate(Value);

        /// <summary>
        /// Returns the specified value replacing the actual one when the contract failed.
        /// </summary>
        /// <param name="newValue">The value to be returned.</param>
        /// <returns>The original value if matches the predicate or the specified value of <typeparamref name="T" /> type.</returns>
        public T Return(T newValue) => IsValid ? Value : newValue;

        /// <summary>
        /// Returns the specified value replacing the actual one when the contract failed.
        /// Be aware to handle any kind of exception.
        /// </summary>
        /// <param name="valueProvider">The delegate that will be used to provide the new value.</param>
        /// <returns>The original value if matches the predicate or the value provided by the delegate of <typeparamref name="T" /> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="valueProvider"/> is null.</exception>
        public T Return(Func<T> valueProvider)
        {
            _ = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
            return IsValid ? Value : valueProvider();
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <returns>An <see cref="ArgumentNullException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowArgumentNullException() => IsValid ? Value : throw new ArgumentNullException(ParameterName);

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An <see cref="ArgumentNullException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowArgumentNullException([NotNull] string message) => IsValid ? Value : throw new ArgumentNullException(ParameterName, message);

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with the message when the contract failed  or
        /// the original value if matches the predicate.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An <see cref="ArgumentException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowArgumentException(string message) => IsValid ? Value : throw new ArgumentException(message, ParameterName);

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowArgumentOutOfRangeException() => IsValid ? Value : throw new ArgumentOutOfRangeException(ParameterName);

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowArgumentOutOfRangeException(string message)
            => IsValid ? Value : throw new ArgumentOutOfRangeException(ParameterName, Value, message);

        /// <summary>
        /// Throws the specified type of exception with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new exception of <typeparamref name="TException" /> type if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public T ThrowException<TException>() where TException : Exception
            => IsValid ? Value : throw this.ExceptionBuilder(ContractExceptionBuilders.ExceptionDelegateBuilder<T, TException>());

        /// <summary>
        /// Throws the specified type of exception from builder with the message when the contract failed or
        /// the original value if matches the predicate.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="exceptionBuilder">A delegate to build an instance of the expected exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type if the value don't match the predicate, otherwise the value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionBuilder"/> is null.</exception>
        [DebuggerStepThrough]
        public T ThrowException<TException>(Func<Contract<T>, TException> exceptionBuilder)
            where TException : Exception
        {
            _ = exceptionBuilder ?? throw new ArgumentNullException(nameof(exceptionBuilder));
            return IsValid ? Value : throw this.ExceptionBuilder(exceptionBuilder);
        }
    }
}