
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

        public string CreateTableQuery()
        {
            return $@"CREATE TABLE dbo.[{configurations.DatabaseConfiguration["MainTable"]}]
				(
					Id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
					[Year] int NULL,
                    [Category] NVARCHAR(255) NULL,
					[Name] NVARCHAR(255) NULL,
					[Birthdate] DATETIME NULL,
					[Birth Place] NVARCHAR(255) NULL,
                    [County] NVARCHAR(255) NULL,
                    [Residence] NVARCHAR(255) NULL,
                    [Field/Language] NVARCHAR(255) NULL,
                    [Prize Name] NVARCHAR(255)  NULL,
					[Motivation] NVARCHAR(2505) NULL,
					CONSTRAINT pk_id PRIMARY KEY(Id),
                );";
        }

        public string InsertRecordsInTable()
        {
            return $"INSERT INTO[dbo].[{configurations.DatabaseConfiguration["MainTable"]}] " +
                        "           ([Year]," +
                        "               [Category]," +
                        "               [Name]," +
                        "               [Birthdate]," +
                        "               [Birth Place]," +
                        "               [County]," +
                        "               [Residence]," +
                        "               [Field/Language]," +
                        "               [Prize Name]," +
                        "               [Motivation])" +
                        "VALUES(@Year,@Category,@Name,@Birthdate,@BirthPlace,@County,@Residence,@FieldLanguage,@PrizeName,@Motivation) ";
        }

        public string TruncateTable()
        {
            return $"TRUNCATE TABLE[dbo].[{configurations.DatabaseConfiguration["MainTable"]}]";
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

        public string GetDataFromDbTable()
        {
            return $"SELECT * FROM [dbo].[{configurations.DatabaseConfiguration["MainTable"]}]";
        }

        //Table columns and sql parameters

    }
}
