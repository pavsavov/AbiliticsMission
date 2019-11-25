
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
                       $"              ([{this.Year}]," +
                       $"               [{this.Category}]," +
                       $"               [{this.Name}]," +
                       $"               [{this.Birthdate}]," +
                       $"               [{this.BirthPlace}]," +
                       $"               [{this.County}]," +
                       $"               [{this.Residence}]," +
                       $"               [{this.FieldLanguage}]," +
                       $"               [{this.PrizeName}]," +
                       $"               [{this.Motivation}])" +
                        $"VALUES(@{nameof(this.Year)},@{nameof(this.Category)},@{nameof(this.Name)},@{nameof(this.Birthdate)},@{nameof(this.BirthPlace)},@{nameof(this.County)},@{nameof(this.Residence)},@{nameof(this.FieldLanguage)},@{nameof(this.PrizeName)},@{nameof(this.Motivation)}) ";
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

        //Table columns
        public string Year { get; private set; } = "Year";
        public string Category { get; private set; } = "Category";
        public string Name { get; private set; } = "Name";
        public string Birthdate { get; private set; } = "Birthdate";
        public string BirthPlace { get; private set; } = "Birth Place";
        public string County { get; private set; } = "County";
        public string Residence { get; private set; } = "Residence";
        public string FieldLanguage { get; private set; } = "Field/Language";
        public string PrizeName { get; private set; } = "Prize Name";
        public string Motivation { get; private set; } = "Motivation";

    }
}
