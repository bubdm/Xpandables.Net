
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Represents a set of methods to read/write objects from a data source when used with <see cref="IDataContext{TEntity}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The Domain object type.</typeparam>
    public interface IEntityAccessor<TEntity> : IDisposable
        where TEntity : Entity
    {
        /// <summary>
        /// Returns an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// The result must be tracked.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an entity of the <typeparamref name="TEntity"/> type that matches the criteria applied on the specified property of <typeparamref name="TParam"/> type
        /// The result must be tracked.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TEntity> FindAsync<TParam>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, CancellationToken cancellationToken = default)
            where TParam : notnull;

        /// <summary>
        /// Returns an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/>".
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default)
            where TResult : notnull;

        /// <summary>
        /// Returns an entity of the <typeparamref name="TResult"/> type that matches the criteria applied on the specified property of <typeparamref name="TParam"/> type.
        /// The result is converted using the <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TResult> FindAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter,
            CancellationToken cancellationToken = default)
            where TParam : notnull
            where TResult : notnull;

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<TEntity?> TryFindAsync(Expression<Func<TEntity, bool>> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return an entity of the <typeparamref name="TEntity"/> type that matches the criteria applied on <paramref name="propertyExpression"/>.
        /// If not found, returns the <see langword="default"/> value of the type.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TEntity?> TryFindAsync<TParam>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, CancellationToken cancellationToken = default)
            where TParam : notnull;

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria using the <paramref name="converter"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria or <see langword="default"/> if not found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        Task<TResult?> TryFindAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tries to return an entity converted to the <typeparamref name="TResult"/> type that matches the criteria applied on <paramref name="propertyExpression"/>
        /// using the <paramref name="converter"/>.
        /// If not found, returns the <see langword="default"/> value of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an object of <typeparamref name="TEntity"/> type that meets the criteria.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The result source contains no elements.</exception>
        Task<TResult?> TryFindAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter,
            CancellationToken cancellationToken = default)
            where TParam : notnull;

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        IAsyncEnumerable<TResult> SelectAsync<TResult>(Expression<Func<TEntity, bool>> criteria,
            Expression<Func<TEntity, TResult>> converter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumerable of <typeparamref name="TResult"/> type that match the criteria and that can be asynchronously enumerated.
        /// If no result found, returns an empty enumerable.
        /// The result is not tracked.
        /// </summary>
        /// <typeparam name="TParam">The type of the model parameter.</typeparam>
        /// <typeparam name="TResult">Anonymous type to be returned.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member to apply criteria on.</param>
        /// <param name="criteria">Defines a set of criteria that entity should meet to be returned.</param>
        /// <param name="converter">Defines the expression to convert n entity to the expected result.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A collection of <typeparamref name="TResult"/> that can be asynchronously enumerated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="criteria"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        IAsyncEnumerable<TResult> SelectAsync<TParam, TResult>(Expression<Func<TEntity, TParam>> propertyExpression,
            Expression<Func<TParam, bool>> criteria, Expression<Func<TEntity, TResult>> converter,
            CancellationToken cancellationToken = default)
            where TParam : notnull;

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
    }
}
