
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
using System.Data;
using System.Linq;
using System.Threading;

using Xpandables.Net.Correlation;
using Xpandables.Net.Creators;
using Xpandables.Net.Data.Elements;

namespace Xpandables.Net.Data.Mappers
{
    /// <summary>
    /// Provides with methods to map a data row or record to an entity.
    /// </summary>
    public interface IDataMapperRow
    {
        private static SpinLock _spinLock;

        /// <summary>
        /// Maps an entity property with the data row or record value.
        /// </summary>
        /// <param name="data">The data row or record to be used.</param>
        /// <param name="targetType">The entity type to act on.</param>
        /// <param name="options">The execution options to act with.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public void Map(object data, Type targetType, DataOptions options)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));
            _ = targetType ?? throw new ArgumentNullException(nameof(targetType));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);

                var stack = new Stack<IDataEntity>();
                var master = EntityBuilder.Build(targetType, options);
                stack.Push(master);

                while (stack.Count > 0)
                {
                    var entity = stack.Pop();
                    MapPrimitiveRows(data, entity, InstanceCreator);
                    if (Entities.TryGetValue(entity.Identity!, out var foundEntity)) entity = foundEntity;

                    foreach (var reference in entity.Properties.Where(property => !property.IsPrimitive))
                    {
                        var nestedEntity = EntityBuilder.Build(reference.Type, options).SetParent(entity.Entity!);
                        stack.Push(nestedEntity);
                    }

                    if (entity.IsNestedEntity)
                        entity.SetElement(entity.Entity!, entity.ParentEntity!, InstanceCreator);
                }

                Entities.AddOrUpdate(master.Identity!, master, (_, value) => value);
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);
            }
        }

        private static void MapPrimitiveRows(object data, IDataEntity entity, IInstanceCreator instanceCreator)
        {
            var primitives = entity.Properties.Where(property => property.IsPrimitive);
            switch (data)
            {
                case IDataRecord dataRecord:
                    foreach (IDataElementSetter primitive in primitives)
                        primitive.SetData(dataRecord, entity.Entity!, instanceCreator);
                    break;
                case DataRow dataRow:
                    foreach (IDataElementSetter primitive in primitives)
                        primitive.SetData(dataRow, entity.Entity!, instanceCreator);
                    break;
            }

            entity.BuildIdentity();
        }

        /// <summary>
        /// Gets the collection of built entities.
        /// </summary>
        CorrelationCollection<string, IDataEntity> Entities { get; }

        /// <summary>
        /// Gets the entity builder.
        /// </summary>
        IDataEntityBuilder EntityBuilder { get; }

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        IInstanceCreator InstanceCreator { get; }
    }

    /// <summary>
    /// Default implementation of <see cref="IDataMapperRow"/>.
    /// </summary>
    public sealed class DataMapperRow : IDataMapperRow
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperRow"/>.
        /// </summary>
        /// <param name="instanceCreator"></param>
        /// <param name="entityBuilder"></param>
        /// <param name="entities"></param>
        public DataMapperRow(IInstanceCreator instanceCreator, IDataEntityBuilder entityBuilder, CorrelationCollection<string, IDataEntity> entities)
        {
            InstanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            EntityBuilder = entityBuilder ?? throw new ArgumentNullException(nameof(entityBuilder));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        /// <summary>
        /// Gets the collection of built entities.
        /// </summary>
        public CorrelationCollection<string, IDataEntity> Entities { get; }

        /// <summary>
        /// Gets the entity builder.
        /// </summary>
        public IDataEntityBuilder EntityBuilder { get; }

        /// <summary>
        /// Gets the instance creator.
        /// </summary>
        public IInstanceCreator InstanceCreator { get; }
    }
}
