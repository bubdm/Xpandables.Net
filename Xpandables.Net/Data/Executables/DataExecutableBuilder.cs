
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
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// This helper class allows the application author to implement the <see cref="IDataExecutable{TResult}"/>
    /// interface without dedicated class.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class DataExecutableBuilder<TResult> : DataExecutable<TResult>
    {
        private readonly Func<DataExecutableContext, Task<TResult>> _executable;

        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableBuilder{TResult}"/> class with the delegate to be used
        /// as <see cref="IDataExecutable{TResult}"/> implementation.
        /// </summary>
        /// <param name="executable">The delegate to be used when the handler will be invoked.
        /// <para>The delegate should match all the behaviors expected in
        /// the <see cref="IDataExecutable{TResult}"/>
        /// method such as thrown exceptions.</para></param>
        /// <exception cref="ArgumentNullException">The <paramref name="executable"/> is null.</exception>
        public DataExecutableBuilder(Func<DataExecutableContext, Task<TResult>> executable)
            => _executable = executable ?? throw new ArgumentNullException(nameof(executable));

        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public override async Task<Optional<TResult>> ExecuteAsync(DataExecutableContext context)
            => await _executable(context).ConfigureAwait(false);
    }
}
