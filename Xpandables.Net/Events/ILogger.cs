
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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// An interface used for writing log events.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Asynchronously registers logs on entry.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        public async Task OnEntryLogAsync(object sender, object argument) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Asynchronously registers logs on exception.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        /// <param name="exception">The handled exception.</param>
        public async Task OnExceptionLogAsync(object sender, object argument, Exception exception) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Asynchronously registers logs on exit.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        /// <param name="result">The execution result for non-void method.</param>
        public async Task OnExitLogAsync(object sender, object argument, object? result = default) => await Task.CompletedTask.ConfigureAwait(false);

        /// <summary>
        /// Registers logs on entry.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        public void OnEntryLog(object sender, object argument) { }

        /// <summary>
        /// Registers logs on exception.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        /// <param name="exception">The handled exception.</param>
        public void OnExceptionLog(object sender, object argument, Exception exception) { }

        /// <summary>
        /// Registers logs on exit.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="argument">The arguments</param>
        /// <param name="result">The execution result for non-void method.</param>
        public void OnExitLog(object sender, object argument, object? result = default) { }
    }
}
