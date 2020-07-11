
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

namespace Xpandables.Net5.Retry
{
    /// <summary>
    /// This interface needs to be implemented by the command/query handler to manage the retry execution.
    /// </summary>
    /// <typeparam name="TArgument">The command/query type.</typeparam>
    public interface IRetryBehaviorHandler<TArgument>
        where TArgument : class
    {
        /// <summary>
        /// This method get called before the retry execution.
        /// </summary>
        /// <param name="argument">The argument used to call the method with.</param>
        /// <param name="context">The retry execution context.</param>
        Task BeforeRetry(TArgument argument, IRetryContext context);
    }
}
