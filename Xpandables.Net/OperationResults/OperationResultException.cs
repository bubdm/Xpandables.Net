
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
using System.Runtime.Serialization;

namespace Xpandables.Net
{
    /// <summary>
    /// Represents an exception that holds an <see cref="IOperationResult"/>.
    /// Usefull when you don't want to return an <see cref="IOperationResult"/>.
    /// </summary>
    [Serializable]
    public class OperationResultException : Exception
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="OperationResultException"/> class that
        /// contains the specified operation result.
        /// </summary>
        /// <param name="result">The result for the exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="result"/> is null.</exception>
        public OperationResultException(IOperationResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        ///<inheritdoc/>
        protected OperationResultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));

            if (info.GetValue(nameof(Result), info.ObjectType) is not { } result)
                throw new ArgumentNullException(nameof(info));

            Result = (IOperationResult)result;
        }

        ///<inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));

            info.SetType(Result.GetType());
            info.AddValue("Result", Result, Result.GetType());

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the operation result for the exception.
        /// </summary>
        public IOperationResult Result { get; }
    }
}
