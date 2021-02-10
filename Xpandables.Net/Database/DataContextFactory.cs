
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

namespace Xpandables.Net.Database
{
    internal sealed class DataContextFactory<TDataContext> : IDataContextFactory<TDataContext>
        where TDataContext : class, IDataContext
    {
        public Func<TDataContext> Factory { get; }

        public DataContextFactory(Func<TDataContext> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Name = typeof(TDataContext).Name;
        }

        public string Name { get; }

        Func<IDataContext> IDataContextFactory.Factory => () => Factory();
    }
}
