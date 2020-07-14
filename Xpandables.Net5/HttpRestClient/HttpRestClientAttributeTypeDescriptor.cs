
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
using System.Linq;

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// A custom descriptor which attaches a <see cref="HttpRestClientAttribute"/> to an instance of a request type
    /// which implements <see cref="IHttpRestClientAttributeProvider"/>
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public sealed class HttpRestClientAttributeTypeDescriptor<TRequest> : CustomTypeDescriptor
        where TRequest : IHttpRestClientAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTypeDescriptor" /> class using a parent custom type descriptor.
        /// </summary>
        /// <param name="parent">The parent custom type descriptor.</param>
        public HttpRestClientAttributeTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent) { }

        /// <summary>
        /// Returns a collection of custom attributes for the type represented by this type descriptor.
        /// </summary>
        /// <returns>An <see cref="AttributeCollection" /> containing the attributes for the type. The default is <see cref="AttributeCollection.Empty" />.</returns>
        public override AttributeCollection GetAttributes()
        {
            //var attrProviderType = typeof(TRequest).GetInterface(typeof(IHttpRestClientAttributeProvider).Name);
            var attrProviderInstance = GetPropertyOwner(GetProperties().Cast<PropertyDescriptor>().First()) as IHttpRestClientAttributeProvider;
            if (attrProviderInstance is { })
            {
                var attrInstance = attrProviderInstance.GetHttpRestClientAttribute();
                var attributes = base.GetAttributes().Cast<Attribute>().ToList();
                TypeDescriptor.AddAttributes(attrProviderInstance, attrInstance);
                attributes.Add(attrInstance);
                return new AttributeCollection(attributes.ToArray());
            }

            return base.GetAttributes();
        }
    }
}
