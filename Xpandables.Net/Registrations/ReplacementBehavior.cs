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

namespace Xpandables.Net.DependencyInjection.Registrations
{
    /// <summary>
    /// Determines how the service will be replaced.
    /// </summary>
    [Flags]
#pragma warning disable CA1714 // Flags enums should have plural names
    public enum ReplacementBehavior
#pragma warning restore CA1714 // Flags enums should have plural names
    {
        /// <summary>
        /// Replace existing services by service type.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Replace existing services by service type (default).
        /// </summary>
        ServiceType = 1,

        /// <summary>
        /// Replace existing services by implementation type.
        /// </summary>
        ImplementationType = 2,

        /// <summary>
        /// Replace existing services by either service- or implementation type.
        /// </summary>
        All = ServiceType | ImplementationType
    }
}
