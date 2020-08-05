
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
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net.DependencyInjection.Interfaces;
using Xpandables.Net.Extensions;

namespace Xpandables.Net.DependencyInjection.Internals
{
    internal class ImplementationTypeFilter : IImplementationTypeFilter
    {
        public ImplementationTypeFilter(IEnumerable<Type> types) => Types = types;

        internal IEnumerable<Type> Types { get; private set; }

        public IImplementationTypeFilter AssignableTo<T>()
        {
            return AssignableTo(typeof(T));
        }

        public IImplementationTypeFilter AssignableTo(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return AssignableToAny(type);
        }

        public IImplementationTypeFilter AssignableToAny(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return AssignableToAny(types.AsEnumerable());
        }

        public IImplementationTypeFilter AssignableToAny(IEnumerable<Type> types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return Where(t => types.Any(t.IsAssignableTo));
        }

        public IImplementationTypeFilter WithAttribute<TAttrobute>()
            where TAttrobute : Attribute
            => WithAttribute(typeof(TAttrobute));

        public IImplementationTypeFilter WithAttribute(Type attributeType)
        {
            if (attributeType is null) throw new ArgumentNullException(nameof(attributeType));
            return Where(t => t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithAttribute<T>(Func<T, bool> predicate) where T : Attribute
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return Where(t => t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter WithoutAttribute<TAttribute>() where TAttribute : Attribute => WithoutAttribute(typeof(TAttribute));

        public IImplementationTypeFilter WithoutAttribute(Type attributeType)
        {
            if (attributeType is null) throw new ArgumentNullException(nameof(attributeType));
            return Where(t => !t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithoutAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return Where(t => !t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter InNamespaceOf<T>() => InNamespaceOf(typeof(T));

        public IImplementationTypeFilter InNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return InNamespaces(types.Select(t => t.Namespace ?? string.Empty));
        }

        public IImplementationTypeFilter InNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            return InNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter InExactNamespaceOf<T>() => InExactNamespaceOf(typeof(T));

        public IImplementationTypeFilter InExactNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return Where(t => types.Any(x => t.IsInExactNamespace(x.Namespace ?? string.Empty)));
        }

        public IImplementationTypeFilter InExactNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            return Where(t => namespaces.Any(t.IsInExactNamespace));
        }

        public IImplementationTypeFilter InNamespaces(IEnumerable<string> namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            return Where(t => namespaces.Any(t.IsInNamespace));
        }

        public IImplementationTypeFilter NotInNamespaceOf<T>() => NotInNamespaceOf(typeof(T));

        public IImplementationTypeFilter NotInNamespaceOf(params Type[] types)
        {
            if (types is null) throw new ArgumentNullException(nameof(types));
            return NotInNamespaces(types.Select(t => t.Namespace ?? string.Empty));
        }

        public IImplementationTypeFilter NotInNamespaces(params string[] namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            return NotInNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter NotInNamespaces(IEnumerable<string> namespaces)
        {
            if (namespaces is null) throw new ArgumentNullException(nameof(namespaces));
            return Where(t => namespaces.All(ns => !t.IsInNamespace(ns)));
        }

        public IImplementationTypeFilter Where(Func<Type, bool> predicate)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            Types = Types.Where(predicate);
            return this;
        }
    }
}
