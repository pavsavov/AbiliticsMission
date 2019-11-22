using Abilitics.Mission.Common;
using System.Data;
using System.Data.SqlClient;


namespace Abilitics.Mission.DatabaseInitialization
{
	public class TableCreator
	{
		private readonly string appConnectionString;
		private readonly string tableName;

		public TableCreator(string appConnectionString, string tableName)
		{
			this.appConnectionString = appConnectionString;
			this.tableName = tableName;
		}

		public void CreateTable()
		{
			//move to sqlquerycontainer
			var cmdText = SqlQueryContainer.CreateDbTable;

			using (var sqlConnection = new SqlConnection(this.appConnectionString))
			{
				using (var sqlCommand = new SqlCommand(cmdText, sqlConnection))
				{
					sqlConnection.Open();
					var result = sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool TableExists()
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.appConnectionString))
			{
				sqlConnection.Open();

				DataTable dTable = sqlConnection.GetSchema("TABLES", new string[] { null, null, this.tableName });

				sqlConnection.Close();
				return dTable.Rows.Count > 0;
			}
		}
	}
}

