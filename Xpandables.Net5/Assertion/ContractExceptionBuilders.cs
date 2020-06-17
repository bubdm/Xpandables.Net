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

namespace System
{
    /// <summary>
    /// Provides with methods contract for building exceptions.
    /// </summary>
    public static class ContractExceptionBuilders
    {
        internal static IInstanceCreator InstanceCreator = new InstanceCreator();

        internal static Func<Contract<T>, ArgumentNullException> BuildArgumentNullException<T>()
            => contract => new ArgumentNullException(contract.ParameterName);

        internal static Func<Contract<T>, ArgumentException> BuildArgumentException<T>()
            => contract => new ArgumentException(contract.ParameterName);

        internal static Func<Contract<T>, ArgumentOutOfRangeException> BuildArgumentOutOfRangeException<T>()
            => contract => new ArgumentOutOfRangeException(contract.ParameterName);

        internal static Func<Contract<T>, InvalidOperationException> BuildInvalidOperationException<T>()
            => contract => new InvalidOperationException(contract.ParameterName);

        static ContractExceptionBuilders() => InstanceCreator.OnException += exceptionCapture => exceptionCapture.Throw();

        /// <summary>
        /// Build an exception of the type-specific.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns></returns>
        public static Func<Contract<T>, TException?> ExceptionDelegateBuilder<T, TException>()
            where TException : Exception
            => contract => InstanceCreator.Create(typeof(TException), contract.ParameterName) as TException;

        /// <summary>
        /// Generic exception builder for contract.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TException">The type of the exception to build.</typeparam>
        /// <param name="contract">The contract instance.</param>
        /// <param name="exceptionCreator">The exception builder.</param>
        /// <returns>An instance of <typeparamref name="TException"/> exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contract"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionCreator"/> is null.</exception>
        public static TException ExceptionBuilder<T, TException>(
            this Contract<T> contract,
            Func<Contract<T>, TException?> exceptionCreator)
            where TException : Exception
        {
            if (contract is null) throw new ArgumentNullException(nameof(contract));
            if (exceptionCreator is null) throw new ArgumentNullException(nameof(exceptionCreator));

            return exceptionCreator.Invoke(contract) ?? throw new ArgumentNullException($"{exceptionCreator} returns a null exception value.");
        }
    }
}
