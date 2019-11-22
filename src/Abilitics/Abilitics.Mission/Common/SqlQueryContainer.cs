
namespace Abilitics.Mission.Common
{
	public class SqlQueryContainer
	{
		public SqlQueryContainer()
		{

		}

		public static string CreateDbTableQuery { get; private set; } = @"CREATE TABLE dbo.Nobel
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

		
		public static string CreateDatabaseQuery { get; private set; } = $"SELECT database_id FROM sys.databases WHERE Name = AbiliticsMission";

	}
}
