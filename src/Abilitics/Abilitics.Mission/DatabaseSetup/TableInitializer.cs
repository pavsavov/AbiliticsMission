using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using Abilitics.Mission.DatabaseSetup;
using System.Data;
using System.Data.SqlClient;


namespace Abilitics.Mission.DatabaseInitialization
{
    public class TableInitializer : Initializer
    {
        private readonly ConfigurationModel configurations;

        public TableInitializer(ConfigurationModel configurations, SqlQueryContainer queryContainer)
            : base(configurations, queryContainer, nameof(TableInitializer))
        {
            this.configurations = configurations;
        }
        
        public override bool CheckDatabaseEntityExists()
        {
            var appConnectionString = configurations.ConnectionStrings["ApplicationConnection"];
            var tableName = configurations.DatabaseConfiguration["MainTable"];

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

