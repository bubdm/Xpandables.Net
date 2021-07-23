
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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Provides with criteria to search for store entities.
    /// This class derives from <see cref="QueryExpression{TSource}"/>.
    /// </summary>
    public sealed class StoreEntityCriteria<TStoreEntity> : QueryExpression<TStoreEntity>
        where TStoreEntity : StoreEntity
    {
        /// <summary>
        /// Returns an of <see cref="StoreEntityCriteria{TEventStoreEntity}"/> with default values.
        /// </summary>
        public static StoreEntityCriteria<TStoreEntity> Default => new();

        /// <summary>
        /// Gets or sets the string representation of the aggregate identifier.
        /// </summary>
        public string? AggregateId { get; init; }

        /// <summary>
        /// Gets or sets the string representation of the aggregate type name.
        /// </summary>
        public string? AggregateTypeName { get; init; }

        /// <summary>
        /// Gets or sets the type name as <see cref="Regex"/> format. If null, all types will be returned.
        /// </summary>
        public string? EventTypeName { get; init; }

        /// <summary>
        /// Gets or sets whether to check for active record or not.
        /// </summary>
        public bool? IsActive { get; init; }

        /// <summary>
        /// Gets or sets whether or not to check for deleted record or not.
        /// </summary>
        public bool? IsDeleted { get; init; }

        /// <summary>
        /// Gets or sets the date to start search on created date. If null, starts from the beginning.
        /// </summary>
        public DateTime? StartCreatedOn { get; init; }

        /// <summary>
        /// Gets or sets the date to end search on created date.
        /// </summary>
        public DateTime? EndCreatedOn { get; init; }

        /// <summary>
        /// Gets or sets the date to start search on updated date.
        /// </summary>
        public DateTime? StartUpdatedOn { get; init; }

        /// <summary>
        /// Gets or sets the date to end search on updated date.
        /// </summary>
        public DateTime? EndUpdatedOn { get; init; }

        /// <summary>
        /// Gets or sets the predicate applied to the event data.
        /// </summary>
        /// <remarks>
        /// For example :
        /// EventDataCriteria = x => x.EventData.RootElement.GetProperty("Version").GetProperty("Value").GetInt64() == version
        /// This is because Version is parsed as "Version": { "Value": 1 }
        /// </remarks>
        public Expression<Func<TStoreEntity, bool>>? DataCriteria { get; init; }

        /// <summary>
        /// Gets the number of entities to be returned.
        /// </summary>
        public int? Size { get; init; }

        /// <summary>
        /// Gets the page number to be returned.
        /// </summary>
        public int? Index { get; init; }

        ///<inheritdoc/>
        public override Expression<Func<TStoreEntity, bool>> GetExpression()
        {
            var expression = QueryExpressionFactory.Create<TStoreEntity>();

            if (EventTypeName is not null)
                expression = expression.And(x => Regex.IsMatch(x.EventTypeName, EventTypeName));
            if (AggregateId is not null)
                expression = expression.And(x => x.AggregateId == AggregateId);
            if (AggregateTypeName is not null)
                expression = expression.And(x => Regex.IsMatch(x.AggregateTypeName, AggregateTypeName));
            if (StartCreatedOn is not null)
                expression = expression.And(x => x.CreatedOn >= StartCreatedOn.Value);
            if (EndCreatedOn is not null)
                expression = expression.And(x => x.CreatedOn <= EndCreatedOn.Value);
            if (StartUpdatedOn is not null)
                expression = expression.And(x => x.UpdatedOn.HasValue && x.UpdatedOn >= StartUpdatedOn.Value);
            if (EndUpdatedOn is not null)
                expression = expression.And(x => x.UpdatedOn.HasValue && x.UpdatedOn <= EndUpdatedOn.Value);
            if (IsActive is not null)
                expression = expression.And(x => x.IsActive == IsActive.Value);
            if (IsDeleted is not null)
                expression = expression.And(x => x.IsDeleted == IsDeleted.Value);
            if (DataCriteria is not null)
                expression = expression.And(DataCriteria);

            return expression;
        }
    }
}
