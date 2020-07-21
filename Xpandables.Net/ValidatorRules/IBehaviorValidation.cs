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
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.ValidatorRules
{
    /// <summary>
    /// A marker interface that allows the command/query class to be decorated with the validation behavior according to the class type :
    /// <see cref="IQuery{TResult}"/> class implementation will be decorated with <see cref="QueryValidatorBehavior{TQuery, TResult}"/> while
    /// <see cref="ICommand"/> class implementation will be decorated with <see cref="CommandValidatorBehavior{TCommand}"/>.
    /// The default validation behavior uses the data annotations validator on validation attributes. You can implement the interface
    /// <see cref="IValidatorRule{T}"/> or derive a class from <see cref="ValidatorRule{TArgument}"/> to customize a validation behavior.
    /// <para></para>
    /// You need to register the expected behavior to the service collections using the appropriate extension method
    /// for the validation behavior and to register all your custom implementations.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IBehaviorValidation { }
#pragma warning restore CA1040 // Avoid empty interfaces
}
