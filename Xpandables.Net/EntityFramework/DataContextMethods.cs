
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;
using System.Linq.Expressions;
using System.Reflection;

using Xpandables.Net.Entities;
using Xpandables.Net.Enumerations;

namespace Xpandables.Net.EntityFramework
{
    public abstract partial class DataContext : DbContext
    {
        private static readonly MethodInfo convertToStringMethodInfo =
        typeof(DataContext).GetMethod(nameof(ConvertEnumerationToString), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly MethodInfo convertToEnumerationMethodInfo =
            typeof(DataContext).GetMethod(nameof(ConvertStringToEnumeration), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static string ConvertEnumerationToString<T>(T enumeration)
            where T : EnumerationType => enumeration.DisplayName;

        private static T ConvertStringToEnumeration<T>(string displayName)
            where T : EnumerationType => EnumerationType.FromDisplayName<T>(displayName)!;

        private static Expression<Func<T, U>> ConverterMethodToLambdaExpression<T, U>(
            MethodInfo methodInfo,
            string argumentName)
        {
            methodInfo = methodInfo.MakeGenericMethod(new Type[] { typeof(T) == typeof(string) ? typeof(U) : typeof(T) });
            var param = Expression.Parameter(typeof(T), argumentName);
            var methodCall = Expression.Call(null, methodInfo, new Expression[] { param });
            return Expression.Lambda<Func<T, U>>(methodCall, new ParameterExpression[] { param });
        }

        /// <summary>
        /// Returns a generic <see cref="ValueConverter{TModel, TProvider}"/> for <see cref="EnumerationType"/>.
        /// To be used with
        /// <see cref="PropertyBuilder{TProperty}.HasConversion(ValueConverter)"/>.
        /// </summary>
        /// <typeparam name="T">Type of enumeration.</typeparam>
        /// <returns>A value converter matching for <typeparamref name="T"/> type.</returns>
        protected static ValueConverter<T, string> EnumerationConverter<T>()
            where T : EnumerationType
        {
            var convertToStringLamda = ConverterMethodToLambdaExpression<T, string>(
                convertToStringMethodInfo,
                "enumeration");
            var convertToEnumerationLamda = ConverterMethodToLambdaExpression<string, T>(
                convertToEnumerationMethodInfo,
                "displayName");

            return new ValueConverter<T, string>(convertToStringLamda, convertToEnumerationLamda);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class
        /// using the specified options. The <see langword="DataContext.OnConfiguring(DbContextOptionsBuilder)"/>
        /// method will still be called to allow further configuration of the options.
        /// </summary>
        /// <param name="contextOptions">The options for this context.</param>
        protected DataContext(DbContextOptions contextOptions)
            : base(contextOptions)
        {
            ChangeTracker.Tracked += (sender, e) =>
              {
                  if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Entity entity)
                      entity.SetCreationDate(DateTime.UtcNow);
              };

            ChangeTracker.StateChanged += (sender, e) =>
              {
                  if (e.NewState == EntityState.Modified && e.Entry.Entity is Entity entity)
                  {
                      entity.SetUpdateDate(DateTime.UtcNow);
                      if (entity.IsDeleted)
                          entity.SetDeleteDate(entity.UpdatedOn!.Value);
                  }
              };
        }
    }
}
