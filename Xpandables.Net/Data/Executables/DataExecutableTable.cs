
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
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return a data table.
    /// </summary>
    public sealed class DataExecutableTable : DataExecutable<DataTable>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public override async Task<Optional<DataTable>> ExecuteAsync(DataExecutableContext context)
        {
            using var dataSet = new DataSet();
            context.ConnectionContext.Command.CommandText =
                context.ConnectionContext.Command.CommandType == CommandType.StoredProcedure
                ? context.Argument.CommandText
                : context.Argument.CommandText.ParseSql();

            DataParameterBuilder.Build(context.ConnectionContext.Command, context.Argument.Parameters?.ToArray());

            if (context.ConnectionContext.Command.CommandType == CommandType.StoredProcedure)
            {
                context.ConnectionContext.Command.CommandTimeout = 0;
                context.ConnectionContext.Command.CommandText = context.Argument.CommandText.Split('@')[0].Trim();
            }

            if (context.ConnectionContext.Command.CommandType == CommandType.Text
                 && context.Argument.Parameters?.All(p => p is DbParameter) == true
                 && context.ConnectionContext.Command.Connection!.IsSqlConnection())
            {
                await context.ConnectionContext.Command.PrepareAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false);
            }

            using var adapter = context.ConnectionContext.DbProviderFactory.CreateDataAdapter()!;
            adapter.SelectCommand = context.ConnectionContext.Command;
            adapter.AcceptChangesDuringFill = false;
            adapter.FillLoadOption = LoadOption.OverwriteChanges;
            adapter.Fill(dataSet);

            if (context.Argument.Options.IsTransactionEnabled)
                await context.ConnectionContext.Command.Transaction!.CommitAsync(context.Argument.Options.CancellationToken).ConfigureAwait(false);

            return (dataSet.Tables.Count > 0) switch
            {
                true => dataSet.Tables[0],
                _ => Optional<DataTable>.Empty()
            };
        }
    }
}
