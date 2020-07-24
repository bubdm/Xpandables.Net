
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
using System.Data;

using Xpandables.Net.Creators;

namespace Xpandables.Net.Data.Elements
{
    /// <summary>
    /// Allows an application author to set an element with a data reader / data row value.
    /// </summary>
    public interface IDataElementSetter
    {
        /// <summary>
        /// Sets the target element with the data record value.
        /// </summary>
        /// <param name="dataRecord">The data record row to be used.</param>
        /// <param name="target">The target instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRecord"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        void SetData(IDataRecord dataRecord, object target, IInstanceCreator instanceCreator);

        /// <summary>
        /// Sets an target element with the data row value.
        /// </summary>
        /// <param name="dataRow">The data row to be used.</param>
        /// <param name="target">The target instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="dataRow"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        void SetData(DataRow dataRow, object target, IInstanceCreator instanceCreator);

        /// <summary>
        /// Sets the target element with the value.
        /// </summary>
        /// <param name="value">The value to be used.</param>
        /// <param name="target">The target element instance to act on.</param>
        /// <param name="instanceCreator">The instance creator to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceCreator" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Setting the element failed. See inner exception.</exception>
        void SetElement(object? value, object target, IInstanceCreator instanceCreator);
    }
}
