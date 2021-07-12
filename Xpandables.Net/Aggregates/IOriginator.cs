﻿/************************************************************************************************************
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

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// This interface is used for the Memento pattern to store and restore the internal state using a <see cref="ISnapShot"/>.
    /// </summary>
    public interface IOriginator
    {
        /// <summary>
        /// Creates a memento containing a snapshot of its current internal state. 
        /// </summary>
        /// <returns>A memento of the underlying instance.</returns>
        IMemento CreateMemento();

        /// <summary>
        /// Restores the state of the underlying instance using the specified memento.
        /// </summary>
        /// <param name="memento">the memento instance to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="memento"/> is null.</exception>
        void SetMemento(IMemento memento);
    }
}
