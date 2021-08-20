
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
using System.ComponentModel;

namespace Xpandables.Net
{
    /// <summary>
    /// Defines a marker interface for state pattern that allows an object to alter 
    /// its behavior when its internal state changes.
    /// </summary>
    /// <remarks>Inherits from <see cref="INotifyPropertyChanged"/>.</remarks>
    public interface IState : INotifyPropertyChanged
    {
        /// <summary>
        /// Allows applying state-specific behavior to the state context at runtime.
        /// </summary>
        /// <param name="stateContext">The target state context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stateContext"/> is null.</exception>
        void EnterState(IStateContext stateContext);
    }

    /// <summary>
    /// Defines a generic marker interface for state pattern that allows an object to alter 
    /// its behavior when its internal state changes for specific type.
    /// </summary>
    /// <typeparam name="TStateContext">The type of the state context.</typeparam>
    /// <remarks>Inherits from <see cref="INotifyPropertyChanged"/>.</remarks>
    public interface IState<TStateContext> : IState
        where TStateContext : class, IStateContext
    {
        /// <summary>
        /// Allows applying state-specific behavior to the state context at runtime.
        /// </summary>
        /// <param name="context">The target state context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        void EnterState(TStateContext context);

        void IState.EnterState(IStateContext stateContext) => EnterState(stateContext.As<TStateContext>());
    }
}
