
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

namespace Xpandables.Net.Logging
{
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Contains properties about argument, result and/or exception with which the handler method has being invoked.
    /// </summary>
    /// <param name="Instance">The object instance on which the method handler is being executed.</param>
    /// <param name="Argument">The arguments with which the method handler has been invoked.</param>
    /// <param name="ReturnValue">The method handler return value if available.</param>
    /// <param name="Exception">The exception thrown by the target method handler.</param>
    public sealed record LoggerArgument(object Instance, object Argument, IOperationResult? ReturnValue, Exception? Exception);
}
