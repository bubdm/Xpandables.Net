
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

using Xpandables.Net.Data.Connections;
using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data.Options
{
    /// <summary>
    /// Contains execution behaviors. Use <see cref="DataOptionsBuilder"/> to build options.
    /// </summary>
    public sealed class DataOptions
    {
        internal DataOptions(IDataConnection connection, bool isTransactionEnabled, IsolationLevel isolationLevel, Func<DataProperty, bool>? conditionalMapping, ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> mappedNames, ConcurrentDictionary<Type, HashSet<string>> notMappedNames, ConcurrentDictionary<Type, DataPropertyConverter> converters, bool isIdentityRetrieved, CancellationToken cancellationToken, Action<Exception>? onException = default)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            IsTransactionEnabled = isTransactionEnabled;
            IsolationLevel = isolationLevel;
            ConditionalMapping = conditionalMapping;
            MappedNames = mappedNames ?? throw new ArgumentNullException(nameof(mappedNames));
            NotMappedNames = notMappedNames ?? throw new ArgumentNullException(nameof(notMappedNames));
            Converters = converters ?? throw new ArgumentNullException(nameof(converters));
            CancellationToken = cancellationToken;
            IsIdentityRetrieved = isIdentityRetrieved;
            OnException = onException;
        }

        /// <summary>
        /// Gets the data connection attached to the current options.
        /// </summary>
        public IDataConnection Connection { get; }

        /// <summary>
        /// Determines whether or not to use transaction. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool IsTransactionEnabled { get; }

        /// <summary>
        /// Determines the isolation level for transaction. Only used if <see cref="IsTransactionEnabled"/> is <see langword="true"/>.
        /// </summary>
        public IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Defines the delegate that determines whether or not a property should be mapped.
        /// Its default behavior return <see langword="true"/>.
        /// </summary>
        public Func<DataProperty, bool>? ConditionalMapping { get; }

        /// <summary>
        /// Determines whether or not the conditional mapping has been defined. The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool IsConditionalMappingEnabled => ConditionalMapping is { };

        /// <summary>
        /// Contains a collection of manual names mapping.
        /// </summary>
        public ConcurrentDictionary<Type, ConcurrentDictionary<string, string>> MappedNames { get; }
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, string>>();

        /// <summary>
        /// Contains a collection of manual names from types that should not to be mapped.
        /// </summary>
        public ConcurrentDictionary<Type, HashSet<string>> NotMappedNames { get; }
            = new ConcurrentDictionary<Type, HashSet<string>>();

        /// <summary>
        /// Contains a collection of converters.
        /// </summary>
        public ConcurrentDictionary<Type, DataPropertyConverter> Converters { get; }
            = new ConcurrentDictionary<Type, DataPropertyConverter>();

        /// <summary>
        /// Contains the cancellation token to be used.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; } = CancellationToken.None;

        /// <summary>
        /// Determines whether or not the filtered delegate has been defined. The default value is <see langword="false"/>
        /// </summary>
        public bool ContainsNotMappedNames => !NotMappedNames.IsEmpty;

        /// <summary>
        /// Determines whether or not to retrieve the newly created identity from SQL command.
        /// </summary>
        public bool IsIdentityRetrieved { get; }

        /// <summary>
        /// Gets the value whether <see cref="OnException"/> event is defined.
        /// </summary>
        public bool IsOnExceptionDefined => OnException is { };

        /// <summary>
        /// Contains the event to raise when exception is handled.
        /// If defined, the process do not throws exception.
        /// </summary>
        public event Action<Exception>? OnException;

        /// <summary>
        /// Raises the <see cref="OnException"/> event.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        public void OnExceptionHandled(Exception exception) => OnException?.Invoke(exception);
    }
}
