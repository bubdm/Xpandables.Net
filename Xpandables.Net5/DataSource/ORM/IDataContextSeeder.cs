
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
namespace System.Design.ORM
{
    /// <summary>
    /// Allows an application author to dynamically seed a data context before it's retrieved.
    /// This is useful when you need a data context not to be empty.
    /// The target data context should be decorated with the <see cref="IBehaviorDataSeed"/> interface and
    /// the class seeder implementation should be
    /// registered to services collections with the extension method <see langword="AddXDataContext{TDataContextAccessor}"/>
    /// using options.
    /// </summary>
    /// <typeparam name="TDataContext">The type of the data context that
    /// implements <see cref="IDataContext"/> and <see cref="IBehaviorDataSeed"/>.</typeparam>
    public interface IDataContextSeeder<TDataContext>
        where TDataContext : IDataContext, IBehaviorDataSeed
    {
        /// <summary>
        /// Seeds the specified data context as you wish.
        /// Warning : Do not throw exception from this method unless it's absolutely necessary.
        /// This method get called by the <see cref="DataContextSeederBehavior{TDataContext}"/>.
        /// </summary>
        /// <param name="dataContext">The data context instance to act on.</param>
        /// <returns>A seeded data context.</returns>
        TDataContext Seed(TDataContext dataContext);
    }
}