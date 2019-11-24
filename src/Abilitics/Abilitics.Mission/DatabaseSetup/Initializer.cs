using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.DatabaseSetup
{
    public abstract class Initializer
    {
        private readonly ConfigurationModel configurations;
        private readonly SqlQueryContainer queryContainer;
        private readonly string derivedType;

        public Initializer(ConfigurationModel configurations, SqlQueryContainer queryContainer, string derivedType)
        {
            this.configurations = configurations;
            this.queryContainer = queryContainer;
            this.derivedType = derivedType;
        }

        public virtual void CreateDatabaseEntity()
        {
            var commandText = String.Empty;
            var connectionString = String.Empty;

            if (this.derivedType == "DatabaseInitializer")
            {
                commandText = queryContainer.CreateDatabaseQuery();
                connectionString = this.configurations.ConnectionStrings["DefaultConnection"];
            }

            if (this.derivedType == "TableInitializer")
            {
                commandText = queryContainer.CreateTableQuery();
                connectionString = this.configurations.ConnectionStrings["ApplicationConnection"];
            }

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var sqlCommand = new SqlCommand(commandText, sqlConnection))
                {
                    sqlConnection.Open();
                    var rowsAffected = sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public abstract bool CheckDatabaseEntityExists();

    }
}
