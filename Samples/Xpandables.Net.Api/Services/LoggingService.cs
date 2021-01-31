
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
using System.Diagnostics;

using Xpandables.Net.Logging;

namespace Xpandables.Net.Api.Services
{
    public sealed class LoggingService : ILoggingHandler
    {
        public void OnEntry(LoggerArgument argument)
        {
            Trace.WriteLine($"On Entry => Handler name : {argument.Instance.GetType().Name} Argument : {argument.Argument.ToJsonString()}");
        }

        public void OnException(LoggerArgument argument)
        {
            Trace.WriteLine($"OnException => Handler name : {argument.Instance.GetType().Name} Argument : {argument.Argument.ToJsonString()}  Exception : {argument.Exception}");
        }

        public void OnExit(LoggerArgument argument)
        {
            Trace.WriteLine($"On Exit => Handler name : {argument.Instance.GetType().Name} Argument : {argument.Argument.ToJsonString()}");
        }

        public void OnSuccess(LoggerArgument argument)
        {
            Trace.WriteLine($"On Success => Handler name : {argument.Instance.GetType().Name} Argument : {argument.Argument.ToJsonString()} Result : {argument.ReturnValue?.Value?.ToJsonString()}");
        }
    }
}
