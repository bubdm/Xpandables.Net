
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
using Xpandables.Net.Expressions;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// Provides with a protected property that holds token claims information of generic type in a security context.
    /// This interface derives from <see cref="IQueryExpression{TSource}"/> interface.
    /// This interface is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TClaims">The type of claims.</typeparam>
    /// <typeparam name="TSource">The type of the data source</typeparam>
    public interface ITokenClaimExpression<TClaims, TSource> : ITokenClaim<TClaims>, IQueryExpression<TSource>
        where TClaims : class
        where TSource : class
    { }
}
