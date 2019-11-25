using System.Data;
using System.Data.SqlClient;
using System;
using System.Data.OleDb;
using Abilitics.Mission.Common;
using Abilitics.Mission.DatabaseInitialization;
using Abilitics.Mission.Configurations;
using System.Linq;
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

            if (!databaseInitializer.CheckDatabaseEntityExists())
            {
                databaseInitializer.CreateDatabaseEntity();
            }

            //Ensure Table is created.
            var mainTableGenerator = new TableInitializer(configurations, sqlQueryContainer);

            if (!mainTableGenerator.CheckDatabaseEntityExists())
            {
                mainTableGenerator.CreateDatabaseEntity();
            }

            try
            {
                DatabaseRecordsImport(configurations, sqlQueryContainer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ApplicationOutput();
            }

        }

        /// <summary>
        /// Logical connection between data load from excel file and data import to database.
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        private static void DatabaseRecordsImport(ConfigurationModel configurations, SqlQueryContainer sqlQueryContainer)
        {
            var loadedDataFromExcelSheet = MapAndLoadFromExcelFile(configurations, sqlQueryContainer);

            ImportToDatabase(configurations, sqlQueryContainer, loadedDataFromExcelSheet);
        }

        /// <summary>
        /// Read and load data from .xlsx file into DataTable collection
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        private static DataTable MapAndLoadFromExcelFile(ConfigurationModel configurations, SqlQueryContainer sqlQueryContainer)
        {
            var excelData = new DataTable();
            var oleDbConnectionString = configurations.ExcelFileConfiguration["FileConnection"];

            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionString))
            {
                oleDbConnection.Open();

                var sheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();


                var columns = new DataColumn[]
                {
                    new DataColumn($"{sqlQueryContainer.Year}",typeof(int)),
                    new DataColumn($"{sqlQueryContainer.Category}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.Name}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.Birthdate}",typeof(DateTime)),
                    new DataColumn($"{sqlQueryContainer.BirthPlace}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.County}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.Residence}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.FieldLanguage}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.FieldLanguage}",typeof(string)),
                    new DataColumn($"{sqlQueryContainer.Motivation}",typeof(string))
                };

                excelData.Columns.AddRange(columns);

                var commandText = sqlQueryContainer.SelectExcelFileSheet(sheet);

                //Map and Fill DataTable type object with records from excel table. 
                using (OleDbDataAdapter oleAdapter = new OleDbDataAdapter(commandText, oleDbConnection))
                {
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Year}", $"{sqlQueryContainer.Year}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Category}", $"{sqlQueryContainer.Category}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Name}", $"{sqlQueryContainer.Name}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Birthdate}", $"{sqlQueryContainer.Birthdate}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.BirthPlace}", $"{sqlQueryContainer.BirthPlace}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.County}", $"{sqlQueryContainer.County}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Residence}", $"{sqlQueryContainer.Residence}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.FieldLanguage}", $"{sqlQueryContainer.FieldLanguage}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.PrizeName}", $"{sqlQueryContainer.PrizeName}");
                    oleAdapter.TableMappings.Add($"{sqlQueryContainer.Motivation}", $"{sqlQueryContainer.Motivation}");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();

                return excelData;
            }
        }

        /// <summary>
        /// Import loaded data from DataTable type collection into database table.
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        /// <param name="excelData">Mapped and loaded DataTable data structure with records from Excel sheet</param>
        private static void ImportToDatabase(ConfigurationModel configurations, SqlQueryContainer sqlQueryContainer, DataTable excelData)
        {
            // Connect to database in order to transfer data from the DataTable type object, loaded with data from.xlsx fil.
            using (SqlConnection sqlConnection = new SqlConnection(configurations.ConnectionStrings["ApplicationConnection"]))
            {
                var queryAllDataInDb = sqlQueryContainer.GetDataFromDbTable();
                var connectionString = configurations.ConnectionStrings["ApplicationConnection"];
                var currentDataInDb = PullData(connectionString, queryAllDataInDb);

                var rows = currentDataInDb.Rows.Count > 0;

                if (rows)
                {
                    var truncateTableQuery = sqlQueryContainer.TruncateTable();

                    using (SqlCommand command = new SqlCommand(truncateTableQuery, sqlConnection))
                    {
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }

                // define INSERT query with parameters
                string query = sqlQueryContainer.InsertRecordsInTable();

                //count excel file rows;
                int rowCounter = 1;

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    foreach (var row in excelData.AsEnumerable())
                    {
                        rowCounter++;

                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Year)}", SqlDbType.Int).Value = row.ItemArray[0] is DBNull ? (object)DBNull.Value : (int)row.ItemArray[0];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Category)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[1] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[1];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Name)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[2] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[2];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Birthdate)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[3] is DBNull ? (object)DBNull.Value : DateFormatter(row.ItemArray[3], rowCounter);
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.BirthPlace)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[4] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[4];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.County)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[5] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[5];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Residence)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[6] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[6];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.FieldLanguage)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[7] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[7];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.PrizeName)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[8] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[8];
                        command.Parameters.Add($"@{nameof(sqlQueryContainer.Motivation)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[9] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[9];

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

        #region UtilityMethods

        /// <summary>
        /// Gets currently loaded records in database table
        /// </summary>
        /// <param name="connectionString">connection parameters for access to MSSQL Database</param>
        /// <param name="query">Select SQL query for retrieving records</param>
        /// <returns></returns>
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
        /// Formats the incomming date-time.
        /// </summary>
        /// <param name="excelValue">date-time value from excel file</param>
        private static DateTime? DateFormatter(object excelDateValue, int rowCounter)
        {
            DateTime dateTime;
            try
            {
                CultureInfo culture = new CultureInfo("en-US");

                var excelValueAsDateTimeTemp = Convert.ToDateTime(excelDateValue);
                var formatedDateTime = Convert.ToDateTime(excelDateValue).ToString("dd-MMM-y");

                var getYearAsFourDigits = culture.Calendar.ToFourDigitYear(excelValueAsDateTimeTemp.Year);

                if (DateTime.TryParseExact(formatedDateTime, "dd-MMM-y", culture, DateTimeStyles.None, out dateTime))
                {
                    var resultNewDate = new DateTime(getYearAsFourDigits, dateTime.Month, dateTime.Day);
                    return resultNewDate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import process was interupted! Invalid data format in column 'Birthdate' at row {rowCounter}. " +
                                  $"Please provide row value in the following valid date-time format: 'DD-MMM-YY'");
                throw;
            }

            return dateTime;

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
        #endregion
    }
}


