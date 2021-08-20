
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
using Xpandables.Net.Correlations;
using Xpandables.Net.Transactions;
using Xpandables.Net.Validators;
using Xpandables.Net.Visitors;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure operations options.
    /// </summary>
    public sealed class HandlerOptions
    {
        /// <summary>
        /// Enables validator behavior to operations that are decorated with the <see cref="IValidatorDecorator"/>.
        /// </summary>
        public HandlerOptions UseValidatorDecorator() => this.With(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to operations that implement the <see cref="IVisitable{TVisitable}"/> interface.
        /// </summary>
        public HandlerOptions UseVisitDecorator() => this.With(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands/events that are decorated with the <see cref="IPersistenceDecorator"/> .
        /// </summary>
        public HandlerOptions UsePersistenceDecorator() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables correlation behavior to operations that are decorated with the <see cref="ICorrelationDecorator"/>.
        /// </summary>
        public HandlerOptions UseCorrelationDecorator() => this.With(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands that are decorated with the <see cref="ITransactionDecorator"/>.
        /// You must provide with an implementation of <see cref="ITransactionScopeProvider"/>.
        /// </summary>
        public HandlerOptions UseTransactionDecorator() => this.With(cq => cq.IsTransactionEnabled = true);

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
    }
}
