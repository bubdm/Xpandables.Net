
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
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Xpandables.Net.Expressions;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Provides with criteria to search for store entities.
    /// This class derive from <see cref="QueryExpression{TSource}"/>.
    /// </summary>
    /// <typeparam name="TStoreEntity">The type of the store entity to be returned.</typeparam>
    public class StoreEntityCriteria<TStoreEntity> : QueryExpression<TStoreEntity>
        where TStoreEntity : StoreEntity
    {
        /// <summary>
        /// Gets the type name as <see cref="Regex"/> format. If null, all types will be returned.
        /// </summary>
        public string? TypeName { get; }

        /// <summary>
        /// Gets the date to start search. If null, starts from the beginning.
        /// </summary>
        public DateTime? Start { get; }

        /// <summary>
        /// Gets the date to end search.
        /// </summary>
        public DateTime? End { get; }

        /// <summary>
        /// Gets the number of entities to be returned.
        /// The default value is 50.
        /// </summary>
        public int Count { get; } = 50;

        ///<inheritdoc/>
        public override Expression<Func<TStoreEntity, bool>> GetExpression()
        {
            var expression = QueryExpressionFactory.Create<TStoreEntity>();
            if (TypeName is not null)
                expression = expression.And(se => Regex.IsMatch(se.TypeName, TypeName));
            if (Start is not null)
                expression = expression.And(se => se.CreatedOn >= Start.Value);
            if (End is not null)
                expression = expression.And(se => se.CreatedOn <= End.Value);

            return expression;
        }
    }
}
