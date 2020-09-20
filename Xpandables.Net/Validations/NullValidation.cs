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
using System.Threading.Tasks;

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// Validator when no explicit registration exist for a given type.
    /// </summary>
    /// <typeparam name="TArgument">Type of argument to be validated.</typeparam>
    public sealed class NullValidation<TArgument> : IValidation<TArgument>
        where TArgument : class
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        public async Task ValidateAsync(TArgument _) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Validate(TArgument _) { }
    }
}