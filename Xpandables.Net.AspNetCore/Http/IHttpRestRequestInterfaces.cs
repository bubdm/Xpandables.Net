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

//using Microsoft.AspNetCore.JsonPatch;

// Waiting for migration to System.Text.Json

namespace Xpandables.Net.Http;

/// <summary>    
/// Provides with a method to retrieve the request patch content for <see cref="BodyFormat.String"/> type.
/// You may use <see cref="ContentType.JsonPatch"/> as content type.
/// </summary>
/// <typeparam name="TDocument">The target object type.</typeparam>
internal interface IPatchRequest<TDocument> : IHttpClientPatchRequest
    where TDocument : class, new()
{
    ///// <summary>
    ///// Returns the patch document.
    ///// </summary>
    //new JsonPatchDocument<TDocument> GetPatchDocument();

    object IHttpClientPatchRequest.GetPatchDocument() => GetPatchDocument();
}
