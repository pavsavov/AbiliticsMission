using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using System.Data;
using System.Data.SqlClient;


namespace Abilitics.Mission.DatabaseInitialization
{
	public class TableCreator
	{
		private readonly ConfigurationModel configurations;
		private readonly SqlQueryContainer queryContainer;

		public TableCreator(ConfigurationModel configurations, SqlQueryContainer queryContainer)
		{
			this.configurations = configurations;
			this.queryContainer = queryContainer;
		}

		public void CreateTable()
		{
			var commandText = queryContainer.CreateDbTableQuery();
			var appConnectionString = configurations.ConnectionStrings["ApplicationConnection"];

			using (var sqlConnection = new SqlConnection(appConnectionString))
			{
				using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
				{
					sqlConnection.Open();
					var result = sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool TableExists()
		{
			var appConnectionString = configurations.ConnectionStrings["ApplicationConnection"];
			var tableName = configurations.DatabaseConfiguration["TableName"];

			using (SqlConnection sqlConnection = new SqlConnection(appConnectionString))
			{
				sqlConnection.Open();

				DataTable dTable = sqlConnection.GetSchema("TABLES", new string[] { null, null, tableName });

				sqlConnection.Close();
				return dTable.Rows.Count > 0;
			}
		}
	}
}

