
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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net5.Data.Executables
{
    /// <summary>
    ///  Executes a stored procedure or query and return the result on a collection of specific type.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    public sealed class DataExecutableMapper<T> : DataExecutable<List<T>>
         where T : class, new()
    {
        private readonly DataMapper _dataMapper;

        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableMapper{T}"/>.
        /// </summary>
        /// <param name="dataMapper">the mapper to be used.</param>
        public DataExecutableMapper(DataMapper dataMapper)
        {
            _dataMapper = dataMapper ?? throw new ArgumentNullException(nameof(dataMapper));
        }

        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="component">The target component instance.</param>
        /// <param name="argument">The target argument instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="component" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="argument" /> is null.</exception>
        public override async Task<List<T>> ExecuteAsync(DataComponent component, DataArgument argument)
        {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandText = component.CommandType == CommandType.StoredProcedure ? argument.Command : argument.Command.ParseSql();
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            component.Command.CommandType = component.CommandType;
            DataParameterBuilder.Build(component.Command, argument.Parameters?.ToArray());

            if (component.CommandType == CommandType.StoredProcedure)
            {
                component.Command.CommandTimeout = 0;
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                component.Command.CommandText = argument.Command.Split('@')[0].Trim();
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            }

            if (component.CommandType == CommandType.Text
                && argument.Parameters?.All(p => p is DbParameter) == true
                && component.Command.Connection.IsSqlConnection())
            {
                component.Command.Prepare();
            }

            var result = new List<T>();
            switch (argument.Options.ReaderOptions)
            {
                case ReaderOption.DataAdapter:
                    using (var dataSet = new DataSet())
                    {
                        component.Adapter.SelectCommand = component.Command;
                        component.Adapter.AcceptChangesDuringFill = false;
                        component.Adapter.FillLoadOption = LoadOption.OverwriteChanges;
                        component.Adapter.Fill(dataSet);

                        if (argument.Options.IsTransactionEnabled)
                            component.Command.Transaction.Commit();

                        using var dataTable = dataSet.Tables[0];
                        result = _dataMapper.Map<T>(dataTable, argument.Options);
                    }

                    break;

                case ReaderOption.DataReader:
                    using (var reader = await component.Command.ExecuteReaderAsync(argument.Options.CancellationToken).ConfigureAwait(false))
                    {
                        result = _dataMapper.Map<T>(GetRecords(), argument.Options);
                        IEnumerable<IDataRecord> GetRecords()
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                    yield return reader;
                            }
                        }
                    }

                    break;
            }

            return result;
        }
    }
}
