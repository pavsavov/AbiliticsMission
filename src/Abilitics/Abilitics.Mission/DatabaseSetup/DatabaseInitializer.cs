using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using System.Data.SqlClient;


namespace Abilitics.Mission.DatabaseInitialization
{
	public class DatabaseInitializer
	{
		private readonly ConfigurationModel configurations;
		private readonly SqlQueryContainer queryContainer;

		public DatabaseInitializer(ConfigurationModel configurations, SqlQueryContainer queryContainer)
		{
			this.configurations = configurations;
			this.queryContainer = queryContainer;
		}

		public void CreateDatabase()
		{
			var commandText = queryContainer.CreateDatabaseQuery();
			var connectionString = this.configurations.ConnectionStrings["DefaultConnection"];

			using (var sqlConnection = new SqlConnection(connectionString))
			{
				using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
				{
					sqlConnection.Open();
					var rowsAffected = sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public bool CheckDatabaseExists()
		{
			//TODO:move to sqlquerycontainer
			string commandText = queryContainer.CheckDatabaseExists();
			var connectionString = this.configurations.ConnectionStrings["DefaultConnection"];

			bool exists = false;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				using (SqlCommand cmd = new SqlCommand(commandText, sqlConnection))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						exists = reader.HasRows;
					}
				}
				sqlConnection.Close();
			}
			return exists;
		}
	}
}
