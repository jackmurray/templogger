using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace TempLoggerService.Api
{
	public class StoredProcedure
	{
		private DbCommand _command;

		public StoredProcedure(DbConnection connection)
		{
			_command = connection.CreateCommand();
		}

		public StoredProcedure(DbConnection connection, string procedureName) : this(connection)
		{
			WithCommand(procedureName);
		}

		public StoredProcedure WithCommand(string procedureName)
        {
			_command.CommandText = procedureName;
			_command.CommandType = System.Data.CommandType.StoredProcedure;
			return this;
        }

		public StoredProcedure WithParameter(SqlParameter param)
        {
			_command.Parameters.Add(param);
			return this;
        }

		public Task<DbDataReader> ExecuteAsync()
        {
			if (_command.Connection.State != System.Data.ConnectionState.Open)
			{
				_command.Connection.Open();
			}

			return _command.ExecuteReaderAsync();
        }
	}
}
