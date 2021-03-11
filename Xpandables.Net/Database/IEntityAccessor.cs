
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Represents a set of methods to read/write objects from a data source when used with <see cref="IDataContext{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public interface IEntityAccessor<TEntity> : IDisposable
        where TEntity : Entity
    {
        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// The result is tracked by default. You can set the <paramref name="isTracked"/> to <see langword="false"/> to disable tracking.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="isTracked">Determines whether or not the entity result is tracked. The default value is <see langword="true"/>.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<TResult?> TryFindAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, bool isTracked = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria applied on <paramref name="propertyExpression"/>
        /// using the <paramref name="converter"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// The result is tracked by default. You can set the <paramref name="isTracked"/> to <see langword="false"/> to disable tracking.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="isTracked">Determines whether or not the entity result is tracked. The default value is <see langword="true"/>.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TResult"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TResult?> TryFindAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, bool isTracked = true,
            CancellationToken cancellationToken = default)
            where TParam : class;

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked by default. You can set the <paramref name="isTracked"/> to <see langword="true"/> to enable tracking.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="isTracked">Determines whether or not the entity result is tracked. The default value is <see langword="false"/>.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        IAsyncEnumerable<TResult> FetchAllAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, bool isTracked = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked by default. You can set the <paramref name="isTracked"/> to <see langword="true"/> to enable tracking.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="isTracked">Determines whether or not the entity result is tracked. The default value is <see langword="false"/>.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        IAsyncEnumerable<TResult> FetchAllAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter, bool isTracked = false,
            CancellationToken cancellationToken = default)
            where TParam : class;

        /// <summary>
        /// Marks the specified entity to be inserted to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks the specified entity to be updated to the data storage on persistence.
        /// </summary>
        /// <param name="entity">The entity to be added and persisted.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an  asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is null.</exception>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists all pending entities to the data storage.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous persist all operation.</returns>
        /// <exception cref="InvalidOperationException">All exceptions related to the operation.</exception>
        /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
        Task PersistAsync(CancellationToken cancellationToken = default);
    }
}
