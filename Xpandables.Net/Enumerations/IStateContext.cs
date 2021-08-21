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

using System.ComponentModel;

namespace Xpandables.Net;

/// <summary>
/// Defines the context interface for state pattern that allows an object to alter 
/// its behavior when its internal state changes.
/// </summary>
/// <remarks>Inherits from <see cref="INotifyPropertyChanged"/>.</remarks>
public interface IStateContext : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the active state in the context.
    /// </summary>
    IState CurrentState { get; }

    /// <summary>
    /// Allows changing the State object at runtime.
    /// </summary>
    /// <param name="state">The target state.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="state"/> is null.</exception>
    internal void TransitionState(IState state);
}

/// <summary>
/// Defines the generic context interface for state pattern that allows an object to alter 
/// its behavior when its internal state changes.
/// </summary>
/// <remarks>Inherits from <see cref="INotifyPropertyChanged"/>.</remarks>
public interface IStateContext<TState> : IStateContext
    where TState : class, IState
{
    /// <summary>
    /// Gets the active state in the context.
    /// </summary>
    new TState CurrentState { get; }

    IState IStateContext.CurrentState { get => CurrentState; }

    /// <summary>
    /// Allows changing the State object at runtime.
    /// </summary>
    /// <param name="state">The target state.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="state"/> is null.</exception>
    void TransitionState(TState state);

    void IStateContext.TransitionState(IState state) => TransitionState(state.As<TState>());
}
