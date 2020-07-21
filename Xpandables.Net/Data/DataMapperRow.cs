
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
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Threading;

using Xpandables.Net.Creators;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with methods to map a data row or record to an entity.
    /// </summary>
    public sealed class DataMapperRow
    {
        private static SpinLock _spinLock;
        private readonly IInstanceCreator _instanceCreator;
        private readonly DataEntityBuilder _entityBuilder;
        private readonly ConcurrentDictionary<string, DataEntity> _entities;

        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperRow"/>.
        /// </summary>
        /// <param name="instanceCreator"></param>
        /// <param name="entityBuilder"></param>
        /// <param name="entities"></param>
        public DataMapperRow(IInstanceCreator instanceCreator, DataEntityBuilder entityBuilder, ConcurrentDictionary<string, DataEntity> entities)
        {
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
            _entityBuilder = entityBuilder ?? throw new ArgumentNullException(nameof(entityBuilder));
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        /// <summary>
        /// Maps an entity property with the data row or record value.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="data">The data row to be used.</param>
        /// <param name="options">The execution options to act with.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public void Map<T>(object data, DataOptions options)
            where T : class, new()
            => Map(data, typeof(T), options);

        /// <summary>
        /// Maps an entity property with the data row or record value.
        /// </summary>
        /// <param name="data">The data row or record to be used.</param>
        /// <param name="entityType">The entity type to act on.</param>
        /// <param name="options">The execution options to act with.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public void Map(object data, Type entityType, DataOptions options)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (entityType is null) throw new ArgumentNullException(nameof(entityType));
            if (options is null) throw new ArgumentNullException(nameof(options));

            var lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);

                var mapperEntity = _entityBuilder.Build(entityType, options);
                DataMapper(data, mapperEntity, options);
                _entities.AddOrUpdate(mapperEntity.Identity, mapperEntity, (_, v) => v);
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);
            }
        }

        private void DataMapper(object data, DataEntity entity, DataOptions options)
        {
            DataMapperPrimitives(data, entity);
            if (_entities.TryGetValue(entity.Identity, out var foundEntity))
                entity = foundEntity;
            DataMapperReferences(data, entity, options);
        }

        private void DataMapperPrimitives(object data, DataEntity entity)
        {
            var primitives = entity.Properties.Where(property => property.IsPrimitive);
            switch (data)
            {
                case IDataRecord dataRecord:
                    foreach (IDataSetter primitive in primitives)
                        primitive.SetData(dataRecord, entity.Entity!, _instanceCreator);
                    break;
                case DataRow dataRow:
                    foreach (IDataSetter primitive in primitives)
                        primitive.SetData(dataRow, entity.Entity!, _instanceCreator);
                    break;
            }

            entity.BuildIdentity();
        }

        private void DataMapperReferences(object data, DataEntity entity, DataOptions options)
        {
            var references = entity.Properties.Where(property => !property.IsPrimitive);
            foreach (var reference in references)
            {
                var nestedEntity = _entityBuilder.Build(reference.Type, options);
                DataMapper(data, nestedEntity, options);
                reference.SetData(nestedEntity.Entity!, entity.Entity!, _instanceCreator);
            }
        }
    }
}
