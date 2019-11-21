using Abilitics.Mission.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.DatabaseInitialization
{
	public class EnsureDatabaseCreated
	{
		public void CreateDatabase()
		{
			var myConnectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Integrated Security=True";
			var cmdText = "CREATE DATABASE AbiliticsMission";
			var dbName = "AbiliticsMission";

			if (DatabaseExists(dbName, myConnectionString))
			{
				using (var sqlConnection = new SqlConnection(myConnectionString))
				{
					using (var sqlCommand = new SqlCommand(cmdText, sqlConnection))
					{
						sqlConnection.Open();
						var rowsAffected = sqlCommand.ExecuteNonQuery();
					}
				}
			}
		}


		/// <summary>
		/// probably wil be reworked
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		private bool DatabaseExists(string databaseName, string connectionString)
		{
			string sqlCreateDBQuery = $"SELECT database_id FROM sys.databases WHERE Name ={databaseName}";
			var result = false;

			try
			{
				using (var sqlConnection = new SqlConnection(connectionString))
				{
					using (var sqlCommand = new SqlCommand(sqlCreateDBQuery, sqlConnection))
					{
						sqlConnection.Open();

						var resultObject = sqlCommand.ExecuteScalar();

						int databaseId = 0;
						if (resultObject != null)
						{
							int.TryParse(resultObject.ToString(), out databaseId);
						}

						result = databaseId > 0;
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
			}

			return result;
		}
	}
}
