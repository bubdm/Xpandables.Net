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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Provides with the base option class for <see cref="IDataLogContext"/> settings.
    /// </summary>
    public sealed class DataLogContextSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataLogContextSettings"/> class.
        /// </summary>
        public DataLogContextSettings() { }

        /// <summary>
        /// Initializes a new instance of <see cref="DataLogContextSettings"/> with properties.
        /// </summary>
        /// <param name="ensuredDeletedBefore">Determine whether or not the database for the context will be deleted before applying migration.</param>
        /// <param name="useInMemory">Determine whether or not the database is uses in memory</param>
        /// <param name="applyMigrations">Determine whether or not the database the migrations must be applied.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionString"/> is null.</exception>
        public DataLogContextSettings(bool ensuredDeletedBefore, bool useInMemory, bool applyMigrations, string connectionString)
        {
            EnsuredDeletedBefore = ensuredDeletedBefore;
            UseInMemory = useInMemory;
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
        /// The database connection string.
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }
}
