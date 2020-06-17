
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
using System.Collections.Generic;

namespace System.Design.Interception
{
    /// <summary>
    /// This interface represents a list of either input or output
    /// parameters. It implements a fixed size list.
    /// </summary>
    public interface IParameterCollection : IEnumerable<Parameter>
    {
        /// <summary>
        /// Fetches a parameter's value by name.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>value of the named parameter.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="parameterName"/> does not exist</exception>
        Parameter this[string parameterName] { get; set; }

        /// <summary>
        /// Fetches a parameter's value by index.
        /// </summary>
        /// <param name="parameterIndex">The parameter index.</param>
        /// <returns>Value of the indexed parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="parameterIndex"/> does not exist</exception>
        Parameter this[int parameterIndex] { get; set; }

        /// <summary>
        /// Does this collection contain a parameter value with the given name?
        /// </summary>
        /// <param name="parameterName">Name of parameter to find.</param>
        /// <returns>True if the parameter name is in the collection, false if not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        bool ContainsParameter(string parameterName);
    }
}
