
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
using System.Text.Json;
using System.Text.RegularExpressions;

using Xpandables.Net.Database;
using Xpandables.Net.Expressions;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Provides with criteria to search for store entities.
    /// This class derives from <see cref="QueryExpression{TSource}"/>.
    /// </summary>
    public sealed class EventStoreEntityCriteria : QueryExpression<EventStoreEntity>
    {
        /// <summary>
        /// Returns an of <see cref="EventStoreEntityCriteria"/> with default values.
        /// </summary>
        public static EventStoreEntityCriteria Default => new();

        /// <summary>
        /// Gets the string representation of the aggregate identifier.
        /// </summary>
        public string? AggregateId { get; init; }

        /// <summary>
        /// Gets the string representation of the aggregate type name.
        /// </summary>
        public string? AggregateTypeName { get; init; }

        /// <summary>
        /// Gets the type name as <see cref="Regex"/> format. If null, all types will be returned.
        /// </summary>
        public string? EventTypeName { get; init; }

        /// <summary>
        /// Determines whether to check for active record or not.
        /// </summary>
        public bool? IsActive { get; init; }

        /// <summary>
        /// Determines whether or not to check for deleted record or not.
        /// </summary>
        public bool? IsDeleted { get; init; }

        /// <summary>
        /// Gets the date to start search on created date. If null, starts from the beginning.
        /// </summary>
        public DateTime? StartCreatedOn { get; init; }

        /// <summary>
        /// Gets the date to end search on created date.
        /// </summary>
        public DateTime? EndCreatedOn { get; init; }

        /// <summary>
        /// Gets the date to start search on updated date.
        /// </summary>
        public DateTime? StartUpdatedOn { get; init; }

        /// <summary>
        /// Gets the date to end search on updated date.
        /// </summary>
        public DateTime? EndUpdatedOn { get; init; }

        /// <summary>
        /// Gets the predicate to apply to the event data.
        /// </summary>
        public Predicate<JsonDocument>? EventDataCriteria { get; init; }

        /// <summary>
        /// Gets the number of entities to be returned.
        /// </summary>
        public int? Count { get; init; }

        ///<inheritdoc/>
        public override Expression<Func<EventStoreEntity, bool>> GetExpression()
        {
            var expression = QueryExpressionFactory.Create<EventStoreEntity>();

            if (EventTypeName is not null)
                expression = expression.And(entity => Regex.IsMatch(entity.EventTypeName, EventTypeName));
            if (AggregateId is not null)
                expression = expression.And(entity => entity.AggregateId == AggregateId);
            if (AggregateTypeName is not null)
                expression = expression.And(entity => Regex.IsMatch(entity.AggregateTypeName, AggregateTypeName));
            if (StartCreatedOn is not null)
                expression = expression.And(entity => entity.CreatedOn >= StartCreatedOn.Value);
            if (EndCreatedOn is not null)
                expression = expression.And(entity => entity.CreatedOn <= EndCreatedOn.Value);
            if (StartUpdatedOn is not null)
                expression = expression.And(entity => entity.UpdatedOn.HasValue && entity.UpdatedOn >= StartUpdatedOn.Value);
            if (EndUpdatedOn is not null)
                expression = expression.And(entity => entity.UpdatedOn.HasValue && entity.UpdatedOn <= EndUpdatedOn.Value);
            if (IsActive is not null)
                expression = expression.And(entity => entity.IsActive == IsActive.Value);
            if (IsDeleted is not null)
                expression = expression.And(entity => entity.IsDeleted == IsDeleted.Value);
            if (EventDataCriteria is not null)
                expression = expression.And(entity => EventDataCriteria(entity.EventData));

            return expression;
        }
    }
}
