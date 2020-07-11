
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
using System.ComponentModel;

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// A generic custom type descriptor that returns the <see cref="HttpRestClientAttributeTypeDescriptor{TRequest}"/>
    /// for the specified type request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public sealed class HttpRestClientAttributeDescriptionProvider<TRequest> : TypeDescriptionProvider
        where TRequest : IHttpRestClientAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptionProvider" /> class using a parent type description provider.
        /// </summary>
        /// <param name="parent">The parent type description provider.</param>
        public HttpRestClientAttributeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent) { }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be <see langword="null" /> if no instance was passed to the <see cref="TypeDescriptor" />.</param>
        /// <returns>An <see cref="ICustomTypeDescriptor" /> that can provide meta-data for the type.</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            => new HttpRestClientAttributeTypeDescriptor<TRequest>(base.GetTypeDescriptor(objectType, instance));
    }
}
