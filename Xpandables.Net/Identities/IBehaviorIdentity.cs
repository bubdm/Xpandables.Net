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

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// A marker interface that allows the command/query class to be filled with an identity. The class should derive from
    /// <see cref="IdentityData"/>, <see cref="IdentityData{TUser}"/> or <see cref="IdentityDataExpression{TUser, TSource}"/>
    /// for a query-bale class. You need to provide with an
    /// implementation for <see cref="IIdentityProvider"/>
    /// and register the expected class using the correct extension method.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IBehaviorIdentity { }
#pragma warning restore CA1040 // Avoid empty interfaces
}
