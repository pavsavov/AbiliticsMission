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
	public abstract class BaseDbEntitySetup
	{
		private readonly ConfigurationModel configurations;
		private readonly SqlQueryContainer queryContainer;

		public BaseDbEntitySetup(ConfigurationModel configurations, SqlQueryContainer queryContainer)
		{
			this.configurations = configurations;
			this.queryContainer = queryContainer;
		}

		public virtual void CreateEntity()
		{
			//TODO:move to sqlquerycontainer
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
	}
}
