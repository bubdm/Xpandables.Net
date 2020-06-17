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
using Microsoft.Extensions.DependencyInjection;
using System.Design.DependencyInjection;
using System.Design.Interception;

namespace System.Design
{
    /// <summary>
    /// A marker interface that allows the class implementation to be intercepted.
    /// You need to register the expected behavior using the appropriate extension method :
    /// <see cref="ServiceCollectionExtensions.AddXInterceptor(IServiceCollection, Type, bool, Reflection.Assembly[])"/> or other method,
    /// and provide an implementation for <see cref="IInterceptor"/>.
    /// </summary>
    public interface IBehaviorInterceptor { }
}
