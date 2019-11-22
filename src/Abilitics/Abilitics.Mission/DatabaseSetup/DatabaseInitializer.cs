using Abilitics.Mission.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.DatabaseInitialization
{
	public class DatabaseInitializer
	{
		private readonly string dbName;
		private readonly string dbDefaultConnectionString;

		public DatabaseInitializer(string dbDefaultConnectionString, string dbName)
		{
			this.dbDefaultConnectionString = dbDefaultConnectionString;
			this.dbName = dbName;
		}
		public void CreateDatabase()
		{
			//move to SQLQueryCOntainer
			var cmdText = "CREATE DATABASE AbiliticsMission";

			using (var sqlConnection = new SqlConnection(this.dbDefaultConnectionString))
			{
				using (var sqlCommand = new SqlCommand(cmdText, sqlConnection))
				{
					sqlConnection.Open();
					var rowsAffected = sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool CheckDatabaseExists()
		{
			//move to SQLQueryCOntainer
			string cmdText = "SELECT * FROM master.dbo.sysdatabases WHERE name ='" + this.dbName + "'";
			bool exists = false;
			using (SqlConnection con = new SqlConnection(this.dbDefaultConnectionString))
			{
				con.Open();
				using (SqlCommand cmd = new SqlCommand(cmdText, con))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						exists = reader.HasRows;
					}
				}
				con.Close();
			}
			return exists;
		}
	}
}
