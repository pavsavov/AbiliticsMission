
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
            return @"CREATE TABLE dbo.am_Nobel
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

    //    public string CreateStagingTable()
    //    {
    //        return @"CREATE TABLE dbo.am_NobelStaging
				//(
				//	Id UNIQUEIDENTIFIER DEFAULT NEWID() NOT NULL,
				//	Year int NULL,
    //                Category NVARCHAR(255) NOT NULL,
				//	Name NVARCHAR(255) NOT NULL,
				//	Birthdate NVARCHAR(255) NULL,
				//	Birth_Place NVARCHAR(255) NULL,
    //                County NVARCHAR(255) NULL,
    //                Residence NVARCHAR(255) NULL,
    //                Field_Language NVARCHAR(255) NULL,
    //                Prize_Name NVARCHAR(255)  NULL,
				//	Motivation NVARCHAR(2505) NOT NULL,
				//	CONSTRAINT pk_id_staging PRIMARY KEY(Id),
    //                CONSTRAINT UC_Motivation_staging UNIQUE(Motivation,Name,Category)
    //            );";
    //    }

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

        public string GetStagingTable()
        {
            return $"[{configurations.DatabaseConfiguration["DatabaseName"]}].[dbo].[{configurations.DatabaseConfiguration["StagingTable"]}]";
        }

        public string GetDataFromDbTable()
        {
            return $"SELECT * FROM [dbo].[{configurations.DatabaseConfiguration["MainTable"]}]";
        }

        //public string InsertUniqueValuesIntoMainTable()
        //{

        //    return $"INSERT INTO {configurations.DatabaseConfiguration["MainTable"]}(Id,Year,Category,Name,Birthdate,Birth_Place,County,Residence,Field_Language,Prize_Name,Motivation) " +
        //           $"SELECT Id,Year,Category,Name,Birthdate,Birth_Place,County,Residence,Field_Language,Prize_Name,Motivation FROM {configurations.DatabaseConfiguration["StagingTable "]}"+ 
        //           $"TRUNCATE TABLE {configurations.DatabaseConfiguration["StagingTable"]}";
        //}
    }
}
