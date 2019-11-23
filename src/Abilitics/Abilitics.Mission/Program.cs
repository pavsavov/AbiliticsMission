using System.Data;
using System.Data.SqlClient;
using System;
using System.Data.OleDb;
using Abilitics.Mission.Common;
using Abilitics.Mission.DatabaseInitialization;
using Abilitics.Mission.Configurations;
using System.Linq;

namespace Abilitics.Mission
{
    class Program
    {
        /// <summary>
        /// Set your path to the ApplicationConfig.json configuration file.
        /// </summary>
        private const string configFilePath = @"E:\Projects\AbiliticsMission\src\Abilitics\Abilitics.Mission\ApplicationConfig.json";

        static void Main(string[] args)
        {
            //Load application configurations from ApplicationConfig.json file
            var configBuilder = new AppConfigurationBuilder(configFilePath);
            var configurations = configBuilder.LoadJsonConfigFile();
            var sqlQueryContainer = new SqlQueryContainer(configurations);

            if (configurations == null)
            {
                Console.WriteLine("JSON loader was not able to map data from configuration file. Check configuration file's path!");
            };

            if (sqlQueryContainer == null)
            {
                Console.WriteLine("Initializing SqlQueryContainer encountered problem. Check configuration file's path!");
            };

            //Ensure database is created.
            var databaseInitializer = new DatabaseInitializer(configurations, sqlQueryContainer);

            if (!databaseInitializer.CheckDatabaseExists())
            {
                databaseInitializer.CreateDatabase();
            }

            //Ensure Table is created.
            var mainTableGenerator = new TableCreator(configurations, sqlQueryContainer, "MainTable");
            var stagingTableGenerator = new TableCreator(configurations, sqlQueryContainer, "StagingTable");

            if (!mainTableGenerator.TableExists())
            {
                mainTableGenerator.CreateTable();
            }

            if (!stagingTableGenerator.TableExists())
            {
                stagingTableGenerator.CreateTable();
            }

            //Import or Update data in database table.I
            ImportToDb(configurations, sqlQueryContainer);
        }

        private static void ImportToDb(ConfigurationModel configurations, SqlQueryContainer sqlQueryContainer)
        {
            var excelData = new DataTable();

            using (OleDbConnection oleDbConnection = new OleDbConnection(configurations.ExcelFileConfiguration["FileConnection"]))
            {
                oleDbConnection.Open();

                var sheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();

                var columns = new DataColumn[]
                {
                    new DataColumn("Year",typeof(int)),
                    new DataColumn("Category",typeof(string)),
                    new DataColumn("Name",typeof(string)),
                    new DataColumn("Birthdate",typeof(string)), //temp
					new DataColumn("Birth Place",typeof(string)),
                    new DataColumn("County",typeof(string)),
                    new DataColumn("Residence",typeof(string)),
                    new DataColumn("Field/Language",typeof(string)),
                    new DataColumn("Prize Name",typeof(string)),
                    new DataColumn("Motivation",typeof(string))
                };

                excelData.Columns.AddRange(columns);

                var commandText = sqlQueryContainer.SelectExcelFileSheet(sheet);

                using (OleDbDataAdapter oleAdapter = new OleDbDataAdapter(commandText, oleDbConnection))
                {
                    oleAdapter.TableMappings.Add("Year", "Year");
                    oleAdapter.TableMappings.Add("Category", "Category");
                    oleAdapter.TableMappings.Add("Name", "Name");
                    oleAdapter.TableMappings.Add("Birthdate", "Birthdate"); //temp
                    oleAdapter.TableMappings.Add("Birth Place", "Birth_Place");
                    oleAdapter.TableMappings.Add("County", "County");
                    oleAdapter.TableMappings.Add("Residence", "Residence");
                    oleAdapter.TableMappings.Add("Field/Language", "Field_Language");
                    oleAdapter.TableMappings.Add("Prize Name", "Prize_Name");
                    oleAdapter.TableMappings.Add("Motivation", "Motivation");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();

                using (SqlConnection sqlConnection = new SqlConnection(configurations.ConnectionStrings["ApplicationConnection"]))
                {
                    using (SqlBulkCopy sqlBulk = new SqlBulkCopy(sqlConnection))
                    {
                        sqlBulk.DestinationTableName = configurations.DatabaseConfiguration["StagingTable"];

                        //Mapping db table columns with Excel columns
                        sqlBulk.ColumnMappings.Add("Year", "Year");
                        sqlBulk.ColumnMappings.Add("Category", "Category");
                        sqlBulk.ColumnMappings.Add("Name", "Name");
                        sqlBulk.ColumnMappings.Add("Birthdate", "Birthdate");
                        sqlBulk.ColumnMappings.Add("Birth Place", "Birth_Place");
                        sqlBulk.ColumnMappings.Add("County", "County");
                        sqlBulk.ColumnMappings.Add("Residence", "Residence");
                        sqlBulk.ColumnMappings.Add("Field/Language", "Field_Language");
                        sqlBulk.ColumnMappings.Add("Prize Name", "Prize_Name");
                        sqlBulk.ColumnMappings.Add("Motivation", "Motivation");

                        sqlConnection.Open();

                        try
                        {
                            DataTable distinctTable = excelData.DefaultView.ToTable(true);
                            sqlBulk.WriteToServer(distinctTable);

                            using (var sqlCommand = new SqlCommand(sqlQueryContainer.InsertUniqueValuesIntoMainTable(), sqlConnection))
                            {
                                sqlCommand.ExecuteNonQuery();
                            }
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    sqlConnection.Close();

                    Console.WriteLine(ApplicationOutput());
                }
            }
        }

        /// <summary>
        /// Sets and returns the Console application's output
        /// </summary>
        /// <returns>Application's source code as a string</returns>
        private static string ApplicationOutput()
        {
            var result = "";

            return result;
        }
    }
}


