
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Database;

namespace Xpandables.Net.Decorators.Persistences
{
    /// <summary>
    /// Provides with persistence decorator implementation for command, notification and aggregate.
    /// </summary>
    public abstract class PersistenceDecoratorBase
    {
        private readonly IDataContext _context;


        /// <summary>
        /// Constructs a new instance of <see cref="PersistenceDecoratorBase"/> with the <see cref="IDataContext"/> instance.
        /// </summary>
        /// <param name="context">The context instance.</param>
        protected PersistenceDecoratorBase(IDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        internal async Task HandleAsync(Task handler, CancellationToken cancellationToken)
        {
            await handler.ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
