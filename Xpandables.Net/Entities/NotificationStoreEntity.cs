
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net.Entities;

/// <summary>
/// Represents the notification event store entity.
/// </summary>
public class NotificationStoreEntity : StoreEntity
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <param name="aggregateTypeName"></param>
    /// <param name="eventTypeFullName"></param>
    /// <param name="eventTypeName"></param>
    /// <param name="eventData"></param>
    /// <param name="exceptionTypeFullName"></param>
    /// <param name="exception"></param>
    [JsonConstructor]
    public NotificationStoreEntity(
        string aggregateId,
        string aggregateTypeName,
        string eventTypeFullName,
        string eventTypeName,
        JsonDocument eventData,
        string? exceptionTypeFullName = default,
        string? exception = default)
        : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, exception, exceptionTypeFullName)
    {

    }
}
