﻿/************************************************************************************************************
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

using Xpandables.Net.Correlations;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net
{
    /// <summary>
    /// A marker interface that allows the handler class implementation to use persistence data across the control flow.
    /// The behavior makes use of an implementation of <see cref="IUnitOfWork"/> in the handler class implementation
    /// to persist data at the end of the control flow only if there is no exception.
    /// In order to control the behavior, you can add the <see cref="ICorrelationDecorator"/> to the command/event class and reference
    /// the <see cref="ICorrelationEvent"/> in the handler class implementation, to defines actions to be applied after the control flow with
    /// <see cref="ICorrelationEvent.PostEvent"/> on success and <see cref="ICorrelationEvent.RollbackEvent"/> on exception.
    /// </summary>
    public interface IPersistenceDecorator { }
}
