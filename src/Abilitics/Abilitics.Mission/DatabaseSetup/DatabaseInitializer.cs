using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using Abilitics.Mission.DatabaseSetup;
using System.Data.SqlClient;


namespace Abilitics.Mission.DatabaseInitialization
{
    public class DatabaseInitializer : Initializer
    {
        private readonly ConfigurationModel configurations;
        private readonly SqlQueryContainer queryContainer;

        public DatabaseInitializer(ConfigurationModel configurations, SqlQueryContainer queryContainer)
            : base(configurations, queryContainer, nameof(DatabaseInitializer))
        {
            this.configurations = configurations;
            this.queryContainer = queryContainer;
        }

        public override bool CheckDatabaseEntityExists()
        {
            var commandText = queryContainer.CheckDatabaseExists();
            var connectionString = this.configurations.ConnectionStrings["DefaultConnection"];

            var exists = false;
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
