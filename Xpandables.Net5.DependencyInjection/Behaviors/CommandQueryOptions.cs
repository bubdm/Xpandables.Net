
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
using Xpandables.Net5.Correlation;
using Xpandables.Net5.EntityFramework;
using Xpandables.Net5.Helpers;
using Xpandables.Net5.Identities;
using Xpandables.Net5.Retry;
using Xpandables.Net5.Transactions;
using Xpandables.Net5.ValidatorRules;
using Xpandables.Net5.VisitorRules;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Defines options to configure command/query handlers.
    /// </summary>
    public sealed class CommandQueryOptions
    {
        /// <summary>
        /// Enables validation behavior to commands and queries that are decorated with the <see cref="IBehaviorValidation"/>.
        /// </summary>
        public CommandQueryOptions UseValidatorBehavior() => this.With(cq => cq.IsValidatorEnabled = true);

        /// <summary>
        /// Enables visitor behavior to commands and queries that implement the <see cref="IVisitable"/> interface.
        /// </summary>
        public CommandQueryOptions UseVisitorBehavior() => this.With(cq => cq.IsVisitorEnabled = true);

        /// <summary>
        /// Enables persistence behavior to commands and queries that are decorated with the <see cref="IBehaviorPersistence"/> .
        /// </summary>
        public CommandQueryOptions UsePersistenceBehavior() => this.With(cq => cq.IsPersistenceEnabled = true);

        /// <summary>
        /// Enables correlation behavior to commands and queries that are decorated with the <see cref="IBehaviorCorrelation"/>.
        /// </summary>
        public CommandQueryOptions UseCorrelationBehavior() => this.With(cq => cq.IsCorrelationEnabled = true);

        /// <summary>
        /// Enables retry behavior to commands and queries that are decorated with the <see cref="IBehaviorRetry"/>.
        /// </summary>
        public CommandQueryOptions UseRetryBehavior() => this.With(cq => cq.IsRetryEnabled = true);

        /// <summary>
        /// Enables transaction behavior to commands and queries that are decorated with the <see cref="IBehaviorTransaction"/>.
        /// </summary>
        public CommandQueryOptions UseTransactionBehavior() => this.With(cq => cq.IsTransactionEnabled = true);

        /// <summary>
        /// Enables identity data behavior to commands and queries that are decorated with the <see cref="IBehaviorIdentity"/>.
        /// </summary>
        public CommandQueryOptions UseIdentityBehavior() => this.With(cq => cq.IsIdentityDataEnabled = true);

        internal bool IsValidatorEnabled { get; private set; }
        internal bool IsVisitorEnabled { get; private set; }
        internal bool IsTransactionEnabled { get; private set; }
        internal bool IsPersistenceEnabled { get; private set; }
        internal bool IsCorrelationEnabled { get; private set; }
        internal bool IsRetryEnabled { get; private set; }
        internal bool? IsIdentityDataEnabled { get; private set; }
    }
}
