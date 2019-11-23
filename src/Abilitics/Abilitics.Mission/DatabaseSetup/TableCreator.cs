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
        private readonly string tableType;

        public TableCreator(ConfigurationModel configurations, SqlQueryContainer queryContainer,string tableType)
        {
            this.configurations = configurations;
            this.queryContainer = queryContainer;
            this.tableType = tableType;
        }

        public void CreateTable()
        {
            string commandText = "";

            if (this.tableType == "MainTable")
            {
                commandText = queryContainer.CreateDbTableQuery();
            }

            if(this.tableType == "StagingTable")
            {
                commandText = queryContainer.CreateStagingTable();

            }

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
            var tableName = configurations.DatabaseConfiguration[this.tableType];

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

