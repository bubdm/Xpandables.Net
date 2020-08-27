
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
using System.Threading;

using Serilog.Core;
using Serilog.Events;

using Xpandables.Net.Extensions;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Provides with a custom emitter for the specified log event type.
    /// <para>Using the <see cref="IDataLogContext"/> interface to save the log.</para>
    /// </summary>
    /// <typeparam name="TLogEntity">The type of the log event.</typeparam>
    public sealed class LogEntitySink<TLogEntity> :ILogEventSink
        where TLogEntity : Entity, ILogEntity<TLogEntity>, new()
    {
        private readonly IDataLogContext _dataContext;
        private static SpinLock _spinLock = new SpinLock();

        /// <summary>
        /// Initializes the <see cref="LogEntitySink{T}"/> with the  data context and the log event converter.
        /// </summary>
        /// <param name="dataLogContext">The data context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataLogContext"/> is null.</exception>
        public LogEntitySink(IDataLogContext<TLogEntity> dataLogContext)
        {
            _dataContext = dataLogContext ?? throw new ArgumentNullException(nameof(dataLogContext));
            _dataContext.OnPersistenceException(exception =>
            {
                System.Diagnostics.Trace.WriteLine(exception);
                return default;
            });
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            var lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (logEvent is null) return;

                var log = new TLogEntity().LoadFrom(logEvent);
                AsyncExtensions.RunSync(_dataContext.AddEntityAsync(log));
                _dataContext.Persist();
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);
            }
        }
    }
}
