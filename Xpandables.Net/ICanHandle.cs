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

namespace Xpandables.Net
{
    /// <summary>
    /// Provides a method that determines whether or not an argument can be handled.
    /// </summary>
    public interface ICanHandle
    {
        /// <summary>
        /// Determines whether or not a an argument can be handled by the underlying context.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// The default behavior returns <see langword="true"/>.
        /// </summary>
        /// <param name="argument">The argument to handle.</param>
        /// <returns><see langword="true"/> if the argument can be handled, otherwise <see langword="false"/></returns>
        public bool CanHandle(object argument) => true;
    }

    /// <summary>
    /// Provides a method that determines whether or not a generic argument can be handled.
    /// </summary>
    /// <typeparam name="TArgument">Type of the argument to handle.</typeparam>
    public interface ICanHandle<in TArgument> : ICanHandle
        where TArgument : class
    {
        /// <summary>
        /// Determines whether or not a type specific argument can be handled by the underlying context.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// The default behavior returns <see langword="true"/>.
        /// </summary>
        /// <param name="argument">The argument to handle.</param>
        /// <returns><see langword="true"/> if the argument can be handled, otherwise <see langword="false"/></returns>
        public bool CanHandle(TArgument argument) => true;

        /// <summary>
        /// Determines whether or not a an argument can be handled by the underlying context.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// The default behavior calls the <see cref="CanHandle(TArgument)"/>.
        /// </summary>
        /// <param name="argument">The argument to handle.</param>
        /// <returns><see langword="true"/> if the argument can be handled, otherwise <see langword="false"/></returns>
        public new sealed bool CanHandle(object argument) => CanHandle((TArgument)argument);
    }
}