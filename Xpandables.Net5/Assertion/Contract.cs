
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

namespace Xpandables.Net5.Assertion
{
    /// <summary>
    /// Allows an application author to check for a condition.
    /// You can use an extension class to add conditions and throw custom exceptions.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public readonly struct Contract<TValue> : IEquatable<Contract<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Contract{T}"/> class with the value to be checked, the
        /// predicate to be applied on this value and the argument name being checked.
        /// </summary>
        /// <param name="value">The value to act on.</param>
        /// <param name="condition">The condition to be applied.</param>
        /// <param name="parameterName">The argument name.</param>
        /// <exception cref="ContractException">The <paramref name="condition"/> is null.</exception>
        /// <exception cref="ContractException">The <paramref name="parameterName"/> is null.</exception>
        public Contract(
            TValue value,
             Func<TValue, bool> condition,
             string parameterName)
        {
            Value = value;
            Condition = condition ?? throw new ContractException(new ArgumentNullException(nameof(condition)));
            ParameterName = parameterName ?? throw new ContractException(new ArgumentNullException(nameof(parameterName)));
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>

        public readonly string ParameterName { get; }

        /// <summary>
        /// Gets the value to check.
        /// </summary>
        public readonly TValue Value { get; }

        /// <summary>
        /// Gets the condition to be applied.
        /// </summary>

        public readonly Func<TValue, bool> Condition { get; }

        /// <summary>
        /// Determine whether or not the value matches the condition.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValid => Condition(Value);

        /// <summary>
        /// Returns the specified value replacing the actual one when the condition fails.
        /// </summary>
        /// <param name="newValue">The value to be returned.</param>
        /// <returns>The original value if matches the condition or the specified value of <typeparamref name="TValue" /> type.</returns>
        [DebuggerStepThrough]
        public TValue Return(TValue newValue) => IsValid ? Value : newValue;

        /// <summary>
        /// Returns the specified value replacing the actual one when the condition fails.
        /// Be aware to handle any kind of exception.
        /// </summary>
        /// <param name="valueProvider">The delegate that will be used to provide the new value.</param>
        /// <returns>The original value if matches the condition or the value provided by the delegate of <typeparamref name="TValue" /> type.</returns>
        /// <exception cref="ContractException">The <paramref name="valueProvider"/> is null.</exception>
        [DebuggerStepThrough]
        public TValue Return(Func<TValue> valueProvider)
        {
            _ = valueProvider ?? throw new ContractException(new ArgumentNullException(nameof(valueProvider)));
            return IsValid ? Value : valueProvider();
        }

        /// <summary>
        /// Returns the original value if condition is valid or throws an <see cref="ArgumentNullException"/>
        /// if the contract fails with the parameter name.
        /// </summary>
        /// <returns>An <see cref="ArgumentNullException"/> if the value don't match the condition, otherwise the value.</returns>
        [DebuggerStepThrough]
        public TValue ThrowArgumentNullException() => IsValid ? Value : throw new ArgumentNullException(ParameterName);

        /// <summary>
        /// Returns the original value if condition is valid or throws an <see cref="ArgumentNullException"/>
        /// if the contract fails with the parameter name and the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>An <see cref="ArgumentNullException"/> if the value don't match the condition, otherwise the value.</returns>
        /// <exception cref="ContractException">The <paramref name="message"/> is null.</exception>
        [DebuggerStepThrough]
        public TValue ThrowArgumentNullException(string message)
        {
            _ = message ?? throw new ContractException(new ArgumentNullException(nameof(message)));
            return IsValid ? Value : throw new ArgumentNullException(ParameterName, message);
        }

        /// <summary>
        /// Returns the original value if condition is valid or throws an <see cref="ArgumentException"/>
        /// if the contract fails with the parameter name and the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>An <see cref="ArgumentException"/> if the value don't match the condition, otherwise the value.</returns>
        /// <exception cref="ContractException">The <paramref name="message"/> is null.</exception>
        [DebuggerStepThrough]
        public TValue ThrowArgumentException(string message)
        {
            _ = message ?? throw new ContractException(new ArgumentNullException(nameof(message)));
            return IsValid ? Value : throw new ArgumentException(message, ParameterName);
        }

        /// <summary>
        /// Returns the original value if condition is valid or throws an <see cref="ArgumentOutOfRangeException"/>
        /// if the contract fails with the parameter name.
        /// </summary>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/> if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public TValue ThrowArgumentOutOfRangeException() => IsValid ? Value : throw new ArgumentOutOfRangeException(ParameterName);

        /// <summary>
        /// Returns the original value if condition is valid or throws an <see cref="ArgumentOutOfRangeException"/>
        /// if the contract fails with the parameter name and the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An <see cref="ArgumentOutOfRangeException"/> if the value don't match the predicate, otherwise the value.</returns>
        /// <exception cref="ContractException">The <paramref name="message"/> is null.</exception>
        [DebuggerStepThrough]
        public TValue ThrowArgumentOutOfRangeException(string message)
        {
            _ = message ?? throw new ContractException(new ArgumentNullException(nameof(message)));
            return IsValid ? Value : throw new ArgumentOutOfRangeException(ParameterName, Value, message);
        }

        /// <summary>
        /// Returns the original value if condition is valid or throws an <typeparamref name="TException"/> exception
        /// if the contract fails with the parameter name.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <returns>A new exception of <typeparamref name="TException" /> type if the value don't match the predicate, otherwise the value.</returns>
        [DebuggerStepThrough]
        public TValue ThrowException<TException>()
            where TException : Exception
            => IsValid ? Value : throw this.ExceptionBuilder(ContractExceptionBuilders.ExceptionDelegateBuilder<TValue, TException>());

        /// <summary>
        /// Returns the original value if condition is valid or throws an <typeparamref name="TException"/> exception
        /// if the contract fails with the parameter name.
        /// </summary>
        /// <typeparam name="TException">Type of exception.</typeparam>
        /// <param name="exceptionBuilder">A delegate to build an instance of the expected exception.</param>
        /// <returns>A new exception of <typeparamref name="TException" /> type if the value don't match the predicate, otherwise the value.</returns>
        /// <exception cref="ContractException">The <paramref name="exceptionBuilder"/> is null.</exception>
        [DebuggerStepThrough]
        public TValue ThrowException<TException>(Func<Contract<TValue>, TException> exceptionBuilder)
            where TException : Exception
        {
            _ = exceptionBuilder ?? throw new ContractException(new ArgumentNullException(nameof(exceptionBuilder)));
            return IsValid ? Value : throw this.ExceptionBuilder(exceptionBuilder);
        }

        /// <summary>
        /// Compares the <see cref="Contract{TValue}"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object? obj) => obj is Contract<TValue> contract && this == contract;

        /// <summary>
        /// Computes the hash-code for the <see cref="Contract{TValue}"/> instance.
        /// </summary>
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(Contract<TValue> left, Contract<TValue> right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(Contract<TValue> left, Contract<TValue> right) => !(left == right);

        /// <summary>
        /// Compares <see cref="EncryptedValue"/> with the value.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(Contract<TValue> other)
            => Value switch
            {
                null when other.Value is null => true,
                { } when other.Value is { } => Value.Equals(other.Value),
                _ => false
            };


    }
}