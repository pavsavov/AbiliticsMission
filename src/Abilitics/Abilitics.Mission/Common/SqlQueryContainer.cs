
using Abilitics.Mission.Configurations;

namespace Abilitics.Mission.Common
{
	public class SqlQueryContainer
	{
		private readonly ConfigurationModel configurations;

		public SqlQueryContainer(ConfigurationModel configurations)
		{
			this.configurations = configurations;
		}

		public string CreateDbTableQuery()
		{
			return @"CREATE TABLE dbo.Nobel
				(
					Id int IDENTITY(1,1) NOT NULL,
					Year int NOT NULL,
                    Category NVARCHAR(255) NOT NULL,
					Name NVARCHAR(255) NOT NULL,
					Birthdate NVARCHAR(255) NULL,
					Birth_Place NVARCHAR(255) NULL,
                    County NVARCHAR(255) NULL,
                    Residence NVARCHAR(255) NULL,
                    Field_Language NVARCHAR(255) NULL,
                    Prize_Name NVARCHAR(255) NOT NULL,
					Motivation NVARCHAR(2505) NOT NULL,
					CONSTRAINT pk_id PRIMARY KEY(Id),
                    CONSTRAINT UC_Motivation UNIQUE(Motivation,Name)
                );";
		}

		public string CheckDatabaseExists()
		{
			return $"SELECT* FROM master.dbo.sysdatabases WHERE name = '{configurations.DatabaseConfiguration["DatabaseName"]}'";
		}

		public string CreateDatabaseQuery()
		{
			return $"CREATE DATABASE {configurations.DatabaseConfiguration["DatabaseName"]}";
		}

		public string SelectExcelFileSheet(string sheet)
		{
			return $"SELECT * FROM [{ sheet}]";
		}

		public string GetDestinationTable()
		{
			return $"[{configurations.DatabaseConfiguration["DatabaseName"]}].[dbo].[{configurations.DatabaseConfiguration["TableName"]}]";
		}

	}
}
