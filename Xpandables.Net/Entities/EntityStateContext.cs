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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// The domain object base implementation that provides an identifier and a key generator for derived class.
    /// The <see cref="EntityStateContext{TState}"/> maintains a reference to an instance 
    /// of a State subclass, which represents the current state of the Context.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}")]
    [Serializable]
    public abstract class EntityStateContext<TState> : Entity, IStateContext<TState>
        where TState : class, IState
    {
        ///<inheritdoc/>
        public TState CurrentState { get; protected set; } = default!;

        /// <summary>
        /// Constructs a new instance of the state context with its initial state.
        /// </summary>
        /// <param name="state">The initial state to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="state"/> is null.</exception>
        protected EntityStateContext(TState state) => TransitionState(state);

        ///<inheritdoc/>
        public void TransitionState(TState state)
        {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            CurrentState = state;
            state.EnterState(this);
        }

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <typeparam name="TProperty">Type of the property selector.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="selector">The expression delegate to retrieve the property name.</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>
        /// if the existing value matches the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        protected bool SetProperty<TValue, TProperty>(ref TValue storage, TValue value, Expression<Func<StateContext<TState>, TProperty>> selector)
            => SetProperty(ref storage, value, GetMemberNameFromExpression(selector));

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="selector">The expression delegate to retrieve the property name.
        /// The expression expected is <see langword="nameof"/> with a delegate.</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>i
        /// f the existing value matched the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="selector"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="selector"/> is not a <see cref="ConstantExpression"/>.</exception>
        protected bool SetProperty<TValue>(ref TValue storage, TValue value, Expression<Func<StateContext<TState>, string>> selector)
            => SetProperty(ref storage, value, GetMemberNameFromExpression(selector));

        /// <summary>
        /// Checks if the property does not match the old one.
        /// If so, sets the property and notifies listeners.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="storage">The current value of the property (the back-end property).</param>
        /// <param name="value">The new value of the property (the value).</param>
        /// <param name="propertyName">The name of the property. Optional (Already known at compile time).</param>
        /// <returns><see langword="true"/>if the value was changed, <see langword="false"/>
        /// if the existing value matches the desired value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null or empty.</exception>
        protected virtual bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            if (EqualityComparer<TValue>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Event raised when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Returns the member name from the expression.
        /// The expression delegate is <see langword="nameof"/>, otherwise the result is null.
        /// </summary>
        /// <param name="nameOfExpression">The expression delegate for the property : <see langword="nameof"/>
        /// with delegate expected.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="nameOfExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="nameOfExpression"/> is
        /// not a <see cref="ConstantExpression"/>.</exception>
        private static string GetMemberNameFromExpression(Expression<Func<StateContext<TState>, string>> nameOfExpression)
        {
            _ = nameOfExpression ?? throw new ArgumentNullException(nameof(nameOfExpression));

            return nameOfExpression.Body is ConstantExpression constantExpression
                ? constantExpression.Value?.ToString() ?? throw new ArgumentException("The member expression is null.")
                : throw new ArgumentException("A member expression is expected.");
        }

        /// <summary>
        /// Returns the member name from the expression if found, otherwise returns null.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member name.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is null.</exception>
        private static string GetMemberNameFromExpression<TProperty>(Expression<Func<StateContext<TState>, TProperty>> propertyExpression)
        {
            _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));

            return (propertyExpression.Body as MemberExpression
                ?? ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression)
                ?.Member.Name ??
                throw new ArgumentException("A member expression is expected.");
        }
    }
}
