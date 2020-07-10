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
using System.Diagnostics.CodeAnalysis;

namespace Xpandables.Net5.Assertion
{
    /// <summary>
    /// Provides with methods contract for building exceptions.
    /// </summary>
    public static class ContractExceptionBuilders
    {
        internal static Func<Contract<TValue>, ArgumentNullException> BuildArgumentNullException<TValue>()
            => contract => new ArgumentNullException(contract.ParameterName);

        internal static Func<Contract<TValue>, ArgumentException> BuildArgumentException<TValue>()
            => contract => new ArgumentException(contract.ParameterName);

        internal static Func<Contract<TValue>, ArgumentOutOfRangeException> BuildArgumentOutOfRangeException<TValue>()
            => contract => new ArgumentOutOfRangeException(contract.ParameterName);

        internal static Func<Contract<TValue>, InvalidOperationException> BuildInvalidOperationException<TValue>()
            => contract => new InvalidOperationException(contract.ParameterName);

        /// <summary>
        /// Returns an exception delegate of the type-specific.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns>An instance of <typeparamref name="TException"/> exception.</returns>
        [return: NotNull]
        public static Func<Contract<TValue>, TException> ExceptionDelegateBuilder<TValue, TException>()
            where TException : Exception
            => contract => (TException)Activator.CreateInstance(typeof(TException), contract.ParameterName)!;

        /// <summary>
        /// Returns an exception of the type-specific from the specified contract.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of the exception to build.</typeparam>
        /// <param name="contract">The contract instance.</param>
        /// <param name="exceptionCreator">The exception builder.</param>
        /// <returns>An instance of <typeparamref name="TException"/> exception.</returns>
        /// <exception cref="ContractException">The <paramref name="exceptionCreator"/> is null.</exception>
        [return: NotNull]
        public static TException ExceptionBuilder<T, TException>(
            this Contract<T> contract,
             Func<Contract<T>, TException> exceptionCreator)
            where TException : Exception
        {
            _ = exceptionCreator ?? throw new ContractException(new ArgumentNullException(nameof(exceptionCreator)));
            return exceptionCreator.Invoke(contract)
                    ?? throw new ContractException(new ArgumentNullException($"{exceptionCreator} returns a null exception value."));
        }
    }
}
