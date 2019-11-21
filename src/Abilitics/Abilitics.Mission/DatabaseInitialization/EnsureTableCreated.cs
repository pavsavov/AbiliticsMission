using Abilitics.Mission.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.DatabaseInitialization
{
	public class EnsureTableCreated
	{
		public void CreateTable()
		{
			var myConnectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Integrated Security=True";
			var tableName = "Nobel";
			if (TableExists(tableName, myConnectionString))
			{
				var cmdText = SqlCommands.CreateDbTable;

				using (var sqlConnection = new SqlConnection(myConnectionString))
				{
					using (var sqlCommand = new SqlCommand(cmdText, sqlConnection))
					{
						sqlConnection.Open();
						var result = sqlCommand.ExecuteNonQuery();
					}
				}
			}
		}

		private static bool TableExists(string TableName, string connectionString)
		{
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();

				DataTable dTable = sqlConnection.GetSchema("TABLES",
							   new string[] { null, null, TableName });

				return dTable.Rows.Count > 0;
			}
		}
	}
}

