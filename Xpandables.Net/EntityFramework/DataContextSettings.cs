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

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// Provides with the base option class for <see cref="IDataContext"/> settings.
    /// </summary>
    public class DataContextSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataContextSettings"/> class.
        /// </summary>
        public DataContextSettings() { }

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextSettings"/> with properties.
        /// </summary>
        /// <param name="ensuredDeletedBefore">Determine whether or not the database for the context will be deleted before applying migration.</param>
        /// <param name="useInMemory">Determine whether or not the database is uses in memory</param>
        /// <param name="addSamplesData">Determine whether or not the database will be filled with samples data.</param>
        /// <param name="applyMigrations">Determine whether or not the database the migrations must be applied.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionString"/> is null.</exception>
        public DataContextSettings(bool ensuredDeletedBefore, bool useInMemory, bool addSamplesData, bool applyMigrations, string connectionString)
        {
            EnsuredDeletedBefore = ensuredDeletedBefore;
            UseInMemory = useInMemory;
            AddSamplesData = addSamplesData;
            ApplyMigrations = applyMigrations;
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Ensures that the database for the context will be deleted before applying migration.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool EnsuredDeletedBefore { get; set; } = true;

        /// <summary>
        /// Ensures that migrations will be applied to the database for the generated context.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool ApplyMigrations { get; set; }

        /// <summary>
        /// Ensures that the database is uses in memory.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool UseInMemory { get; set; }

        /// <summary>
        /// Allows the database to be filled with samples data.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool AddSamplesData { get; set; }

        /// <summary>
        /// The database connection string.
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }
}
