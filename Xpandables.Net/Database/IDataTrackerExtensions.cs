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

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Provides with extension methods for <see cref="IDataTracker"/>.
    /// </summary>
    public static class IDataTrackerExtensions
    {
        /// <summary>
        /// Returns the same instance where the result of queries will be tracked for changes.
        /// </summary>
        /// <typeparam name="TDataSource">The data source type.</typeparam>
        /// <param name="dataSource">The target instance to act on.</param>
        /// <returns>The same instance with entities tracking changes enabled.</returns>
        public static TDataSource AsTracking<TDataSource>(this TDataSource dataSource)
            where TDataSource : class, IDataTracker
        {
            dataSource.IsTracked = true;
            return dataSource;
        }

        /// <summary>
        /// Returns the same instance where the result of queries will be either tracked for changes or not.
        /// </summary>
        /// <typeparam name="TDataSource">The data source type.</typeparam>
        /// <param name="dataSource">The target instance to act on.</param>
        /// <param name="enableOrDisableTracking">Enable or disable the entities tracking.</param>
        /// <returns>The same instance with entities tracking changes enabled or not.</returns>
        public static TDataSource AsTracking<TDataSource>(this TDataSource dataSource, bool enableOrDisableTracking)
            where TDataSource : class, IDataTracker
        {
            dataSource.IsTracked = enableOrDisableTracking;
            return dataSource;
        }

        /// <summary>
        /// Returns the same instance where the result of queries will be not tracked.
        /// </summary>
        /// <typeparam name="TDataSource">The data source type.</typeparam>
        /// <param name="dataSource">The target instance to act on</param>
        /// <returns>The same instance with entities tracking changes disabled.</returns>
        public static TDataSource AsNoTracking<TDataSource>(this TDataSource dataSource)
            where TDataSource : class, IDataTracker
        {
            dataSource.IsTracked = false;
            return dataSource;
        }
    }
}
