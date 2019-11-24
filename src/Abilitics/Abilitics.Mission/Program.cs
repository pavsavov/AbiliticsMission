using System.Data;
using System.Data.SqlClient;
using System;
using System.Data.OleDb;
using Abilitics.Mission.Common;
using Abilitics.Mission.DatabaseInitialization;
using Abilitics.Mission.Configurations;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

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
            //var stagingTableGenerator = new TableCreator(configurations, sqlQueryContainer, "StagingTable");

            if (!mainTableGenerator.TableExists())
            {
                mainTableGenerator.CreateTable();
            }

            //if (!stagingTableGenerator.TableExists())
            //{
            //    stagingTableGenerator.CreateTable();
            //}

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
                    oleAdapter.TableMappings.Add("Birth Place", "Birth Place");
                    oleAdapter.TableMappings.Add("County", "County");
                    oleAdapter.TableMappings.Add("Residence", "Residence");
                    oleAdapter.TableMappings.Add("Field/Language", "Field/Language");
                    oleAdapter.TableMappings.Add("Prize Name", "Prize Name");
                    oleAdapter.TableMappings.Add("Motivation", "Motivation");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();

                using (SqlConnection sqlConnection = new SqlConnection(configurations.ConnectionStrings["ApplicationConnection"]))
                {
                    ////using (SqlBulkCopy sqlBulk = new SqlBulkCopy(sqlConnection))
                    //{
                    //    sqlBulk.DestinationTableName = configurations.DatabaseConfiguration["MainTable"];

                    //    //Mapping db table columns with Excel columns
                    //    sqlBulk.ColumnMappings.Add("Year", "Year");
                    //    sqlBulk.ColumnMappings.Add("Category", "Category");
                    //    sqlBulk.ColumnMappings.Add("Name", "Name");
                    //    sqlBulk.ColumnMappings.Add("Birthdate", "Birthdate");
                    //    sqlBulk.ColumnMappings.Add("Birth Place", "Birth Place");
                    //    sqlBulk.ColumnMappings.Add("County", "County");
                    //    sqlBulk.ColumnMappings.Add("Residence", "Residence");
                    //    sqlBulk.ColumnMappings.Add("Field/Language", "Field/Language");
                    //    sqlBulk.ColumnMappings.Add("Prize Name", "Prize Name");
                    //    sqlBulk.ColumnMappings.Add("Motivation", "Motivation");

                    //    sqlConnection.Open();
                    //    // -------------------------UPDATE ZONE --------------------------------
                    var queryAllDataInDb = sqlQueryContainer.GetDataFromDbTable();

                    var currentDataInDb = PullData(configurations.ConnectionStrings["ApplicationConnection"], queryAllDataInDb);

                    //    //map columns for compare
                    //    DataView dbView = new DataView(currentDataInDb);
                    //    DataTable distinctDbData = dbView.ToTable(true, new string[] { "Year", "Category", "Name", "Birthdate", "Birth Place", "County", "Residence", "Field/Language", "Prize Name", "Motivation" });

                    //    //data from excel file - ensure only unique records to be persisted
                    //    DataView excelView = new DataView(excelData);
                    //    DataTable distinctExcel = excelView.ToTable(true, new string[] { "Year", "Category", "Name", "Birthdate", "Birth Place", "County", "Residence", "Field/Language", "Prize Name", "Motivation" });

                    //    //var distinctDbSet = new HashSet<DataRow>();
                    //    //var distinctExcelSet = new HashSet<DataRow>();
                    //    //var updatedRecords = new HashSet<DataRow>();


                    //    sqlBulk.(distinctExcel);

                    //    // -------------------------UPDATE ZONE --------------------------------

                    //}
                    //check if table is populated


                    var rows = currentDataInDb.Rows.Count > 0;

                    if (rows)
                    {
                        var truncateTableQuery = "TRUNCATE TABLE [dbo].[am_Nobel]";
                        using (SqlCommand command = new SqlCommand(truncateTableQuery, sqlConnection))
                        {
                            sqlConnection.Open();
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }

                    // define INSERT query with parameters
                    string query = "INSERT INTO [dbo].[am_Nobel] " +
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
                    int rowCounter = 1;
                    // create command
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        foreach (var row in excelData.AsEnumerable())
                        {
                            rowCounter++;
                            // define parameters and their values
                            command.Parameters.Add("@Year", SqlDbType.Int).Value = row.ItemArray[0] is DBNull ? (object)DBNull.Value : (int)row.ItemArray[0];
                            command.Parameters.Add("@Category", SqlDbType.NVarChar, 255).Value = row.ItemArray[1] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[1];
                            command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = row.ItemArray[2] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[2];
                            command.Parameters.Add("@Birthdate", SqlDbType.NVarChar, 255).Value = row.ItemArray[3] is DBNull ? (object)DBNull.Value : DateFormatter(row.ItemArray[3], rowCounter);
                            command.Parameters.Add("@BirthPlace", SqlDbType.NVarChar, 255).Value = row.ItemArray[4] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[4];
                            command.Parameters.Add("@County", SqlDbType.NVarChar, 255).Value = row.ItemArray[5] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[5];
                            command.Parameters.Add("@Residence", SqlDbType.NVarChar, 255).Value = row.ItemArray[6] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[6];
                            command.Parameters.Add("@FieldLanguage", SqlDbType.NVarChar, 255).Value = row.ItemArray[7] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[7];
                            command.Parameters.Add("@PrizeName", SqlDbType.NVarChar, 255).Value = row.ItemArray[8] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[8];
                            command.Parameters.Add("@Motivation", SqlDbType.NVarChar, 255).Value = row.ItemArray[9] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[9];

                            // open connection, execute UPDATE, close connection
                            sqlConnection.Close();
                            sqlConnection.Open();
                            command.ExecuteNonQuery();
                            sqlConnection.Close();

                            command.Parameters.Clear();

                        }
                    }

                    Console.WriteLine(ApplicationOutput());
                }
            }
        }
        #region UtilityMethods

        /// <summary>
        /// Sets and returns the Console application's output
        /// </summary>
        /// <returns>Application's source code as a string</returns>
        private static string ApplicationOutput()
        {
            var result = "";

            return result;
        }

        private static DataTable PullData(string connectionString, string query)
        {
            var dataTable = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataAdapter dbAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        dbAdapter.Fill(dataTable);

                        sqlConnection.Close();

                        dbAdapter.Dispose();
                    }
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Formats the dateTime according to input values;
        /// </summary>
        /// <param name="excelValue"></param>
        /// <returns></returns>

        private static DateTime? DateFormatter(object excelDateValue, int rowCounter)
        {
            DateTime dateTime;
            try
            {
                var result = Convert.ToDateTime(excelDateValue).ToString("dd-MMM-y");

                if (DateTime.TryParseExact(result, "dd-MMM-y", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import process was not successful! Invalid data format in column 'Birthdate' at row {rowCounter}. " +
                                  $"Please provde row value in the following valid format: 'DD-MMM-YY'");
                throw;
            }
            //finally
            //{
            //    Console.WriteLine($"Success! The imported dateTime type value is in correct format.");
            //}

            return dateTime;

        }
        #endregion
    }
}


