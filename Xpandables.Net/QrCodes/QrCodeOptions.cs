
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

using Xpandables.Net.Extensions;
using Xpandables.Net.QrCodes;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Defines options to configure qr-code engine.
    /// </summary>
    public sealed class QrCodeOptions
    {
        /// <summary>
        /// Defines a custom type for image generator.
        /// The type will resolved from the system services provider.
        /// </summary>
        public QrCodeOptions UseImageGenerator<TImageGenerator>()
            where TImageGenerator : class, IQrCodeImageGenerator
            => this.With(qc => qc.IsCustomImageGenerator = typeof(TImageGenerator));

        /// <summary>
        /// Defines a custom type for text generator.
        /// The type will resolved from the system services provider.
        /// </summary>
        public QrCodeOptions UseTextGeneraor<TTextGenerator>()
            where TTextGenerator : class, IQrCodeTextGenerator
            => this.With(qc => qc.IsCustomTextGenerator = typeof(TTextGenerator));

        /// <summary>
        /// Defines a custom type for code validator.
        /// The type will resolved from the system services provider.
        /// </summary>
        public QrCodeOptions UseCodeValidator<TCodeValidator>()
            where TCodeValidator : class, IQrCodeValidator
            => this.With(qc => qc.IsCustomValidator = typeof(TCodeValidator));

        internal Type? IsCustomImageGenerator { get; private set; }
        internal Type? IsCustomTextGenerator { get; private set; }
        internal Type? IsCustomValidator { get; private set; }
    }
}
