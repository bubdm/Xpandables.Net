
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

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// The logical signatures interface.
    /// </summary>
    internal interface ILogicalSignatures
    {
        void F(bool x, bool y);
        void F(bool? x, bool? y);
    }

    /// <summary>
    /// The arithmetic signatures interface.
    /// </summary>
    internal interface IArithmeticSignatures
    {
        void F(int x, int y);
        void F(uint x, uint y);
        void F(long x, long y);
        void F(ulong x, ulong y);
        void F(float x, float y);
        void F(double x, double y);
        void F(decimal x, decimal y);
        void F(int? x, int? y);
        void F(uint? x, uint? y);
        void F(long? x, long? y);
        void F(ulong? x, ulong? y);
        void F(float? x, float? y);
        void F(double? x, double? y);
        void F(decimal? x, decimal? y);
    }

    /// <summary>
    /// The relational signatures interface.
    /// </summary>
    internal interface IRelationalSignatures : IArithmeticSignatures
    {
        void F(string x, string y);
        void F(char x, char y);
        void F(DateTime x, DateTime y);
        void F(TimeSpan x, TimeSpan y);
        void F(char? x, char? y);
        void F(DateTime? x, DateTime? y);
        void F(TimeSpan? x, TimeSpan? y);
    }

    /// <summary>
    /// The equality signatures interface.
    /// </summary>
    internal interface IEqualitySignatures : IRelationalSignatures
    {
        void F(bool x, bool y);
        void F(bool? x, bool? y);
    }

    /// <summary>
    /// The add signatures interface.
    /// </summary>
    internal interface IAddSignatures : IArithmeticSignatures
    {
        void F(DateTime x, TimeSpan y);
        void F(TimeSpan x, TimeSpan y);
        void F(DateTime? x, TimeSpan? y);
        void F(TimeSpan? x, TimeSpan? y);
    }

    /// <summary>
    /// The subtract signatures interface.
    /// </summary>
    internal interface ISubtractSignatures : IAddSignatures
    {
        void F(DateTime x, DateTime y);
        void F(DateTime? x, DateTime? y);
    }

    /// <summary>
    /// The negation signatures interface.
    /// </summary>
    internal interface INegationSignatures
    {
        void F(int x);
        void F(long x);
        void F(float x);
        void F(double x);
        void F(decimal x);
        void F(int? x);
        void F(long? x);
        void F(float? x);
        void F(double? x);
        void F(decimal? x);
    }

    /// <summary>
    /// The not signatures interface.
    /// </summary>
    internal interface INotSignatures
    {
        void F(bool x);
        void F(bool? x);
    }

    /// <summary>
    /// The enumerable signatures interface.
    /// </summary>
    internal interface IEnumerableSignatures
    {
        void Contains(object selector);
        void Where(bool predicate);
        void Any();
        void Any(bool predicate);
        void All(bool predicate);
        void Count();
        void Count(bool predicate);
        void Min(object selector);
        void Max(object selector);
        void Sum(int selector);
        void Sum(int? selector);
        void Sum(long selector);
        void Sum(long? selector);
        void Sum(float selector);
        void Sum(float? selector);
        void Sum(double selector);
        void Sum(double? selector);
        void Sum(decimal selector);
        void Sum(decimal? selector);
        void Average(int selector);
        void Average(int? selector);
        void Average(long selector);
        void Average(long? selector);
        void Average(float selector);
        void Average(float? selector);
        void Average(double selector);
        void Average(double? selector);
        void Average(decimal selector);
        void Average(decimal? selector);
    }
}
