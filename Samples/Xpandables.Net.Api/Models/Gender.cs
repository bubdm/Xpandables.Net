
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
using System.ComponentModel;

using Xpandables.Net.Enumerations;

namespace Xpandables.Net.Api.Models
{
    [TypeConverter(typeof(EnumerationTypeConverter))]
    public sealed class Gender : EnumerationType
    {
        private Gender(string displayName, int value) : base(displayName, value) { }
        public static Gender Man => new Gender(nameof(Man), 0);
        public static Gender Woman => new Gender(nameof(Woman), 1);
        public bool IsMan() => this == Man;
        public bool IsWoman() => this == Woman;
    }
}
