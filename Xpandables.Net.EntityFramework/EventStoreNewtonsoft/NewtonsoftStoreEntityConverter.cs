
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

using Newtonsoft.Json;

using Xpandables.Net.Database;

namespace Xpandables.Net.EntityFramework.EventStoreNewtonsoft
{
    /// <summary>
    /// Event Store converter using <see cref="Newtonsoft.Json"/>.
    /// </summary>
    public sealed class NewtonsoftStoreEntityConverter : IStoreEntityConverter
    {
        ///<inheritdoc/>
        public object Deserialize(string value, Type returnType)
        {
            JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject(value, returnType, settings)!;
        }

        ///<inheritdoc/>
        public string Serialize(object value, Type inputType)
        {
            JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.SerializeObject(value, inputType, settings);
        }
    }
}
