
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

namespace Xpandables.Net.Enqueues
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEnqueueCommand"/> interface.
    /// </summary>
    public abstract class EnqueueCommand : IEnqueueCommand
    {
        private EnqueueStatus _status;

        /// <summary>
        /// Constructs a new instance of <see cref="EnqueueCommand"/> with the target type.
        /// The <see cref="IEnqueueCommand.Status"/> is set to <see cref="EnqueueStatus.NewlyAdded"/>.
        /// </summary>
        /// <param name="enqueueType">The target enqueue type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="enqueueType"/> is null.</exception>
        protected EnqueueCommand(string enqueueType)
        {
            EnqueueType = enqueueType ?? throw new ArgumentNullException(nameof(enqueueType));
            _status = EnqueueStatus.NewlyAdded;
        }

        /// <summary>
        /// Gets the enqueue type.
        /// </summary>
        public string EnqueueType { get; }

        EnqueueStatus IEnqueueCommand.Status { get => _status; set => _status = value; }
    }
}
