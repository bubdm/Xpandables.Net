
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;

using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data.Options
{
    /// <summary>
    ///  Represents a set of values data base executable options properties.
    /// </summary>
    public interface IDataExecutableOptions
    {
        /// <summary>
        /// Gets the value indicating whether or not to use transaction. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        bool IsTransactionEnabled { get; }

        /// <summary>
        /// Gets the isolation level for transaction. Only used if <see cref="IsTransactionEnabled"/> is <see langword="true"/>.
        /// </summary>
        IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Defines the delegate that determines whether or not a property should be mapped.
        /// Its default behavior return <see langword="true"/>.
        /// </summary>
        Func<DataProperty, bool>? ConditionalMapping { get; }

        /// <summary>
        /// Gets the value indicating whether or not the conditional mapping has been defined. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool IsConditionalMappingEnabled => ConditionalMapping is { };

        /// <summary>
        /// Contains a collection of manual names mapping.
        /// </summary>
        ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> MappedNames { get; }

        /// <summary>
        /// Contains a collection of manual names from types that should not to be mapped.
        /// </summary>
        ConcurrentDictionary<Type, HashSet<string>> NotMappedNames { get; }

        /// <summary>
        /// Contains a collection of converters.
        /// </summary>
        ConcurrentDictionary<Type, DataPropertyConverter> Converters { get; }

        /// <summary>
        /// Contains the cancellation token to be used.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets the value indicating whether or not the filtered delegate has been defined. The default value is <see langword="false"/>
        /// </summary>
        public bool ContainsNotMappedNames => !NotMappedNames.IsEmpty;

        /// <summary>
        /// Gets the value indicating whether or not to retrieve the newly created identity from SQL command.
        /// </summary>
        bool IsIdentityRetrieved { get; }

        /// <summary>
        /// Gets the value indicating whether <see cref="OnException"/> event is defined.
        /// </summary>
        bool IsOnExceptionDefined { get; }

        /// <summary>
        /// Contains the event to raise when exception is handled.
        /// If defined, the process do not throws exception.
        /// </summary>
        event Action<Exception>? OnException;

        /// <summary>
        /// Raises the <see cref="OnException"/> event.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        void OnExceptionHandled(Exception exception);
    }
}