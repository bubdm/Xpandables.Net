
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
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace System.Design.SQL.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return a data table.
    /// </summary>
    public sealed class DataExecutableTable : DataExecutable<DataTable>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="component">The target component instance.</param>
        /// <param name="argument">The target argument instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="component" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public override async Task<DataTable> ExecuteAsync(DataComponent component, DataArgument argument)
        {
            using var dataSet = new DataSet();
            component.Command.CommandText = component.CommandType == CommandType.StoredProcedure ? argument.Command : argument.Command.ParseSql();
            component.Command.CommandType = component.CommandType;
            DataParameterBuilder.Build(component.Command, argument.Parameters?.ToArray());

            if (component.CommandType == CommandType.StoredProcedure)
            {
                component.Command.CommandTimeout = 0;
                component.Command.CommandText = argument.Command.Split('@')[0].Trim();
            }

            if (component.CommandType == CommandType.Text
                 && argument.Parameters?.All(p => p is DbParameter) == true
                 && component.Command.Connection.IsSqlConnection())
            {
                component.Command.Prepare();
            }

            component.Adapter.SelectCommand = component.Command;
            component.Adapter.AcceptChangesDuringFill = false;
            component.Adapter.FillLoadOption = LoadOption.OverwriteChanges;
            component.Adapter.Fill(dataSet);

            var result = dataSet.Tables[0];

            if (argument.Options.IsTransactionEnabled)
                component.Command.Transaction.Commit();

            return await Task.FromResult(result).ConfigureAwait(false);
        }
    }
}
