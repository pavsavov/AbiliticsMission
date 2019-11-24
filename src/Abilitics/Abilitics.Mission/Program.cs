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
                ImportToDb(configurations, sqlQueryContainer);
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
                    new DataColumn("Birthdate",typeof(DateTime)), 
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
                    oleAdapter.TableMappings.Add("Birthdate", "Birthdate"); 
                    oleAdapter.TableMappings.Add("Birth Place", "Birth Place");
                    oleAdapter.TableMappings.Add("County", "County");
                    oleAdapter.TableMappings.Add("Residence", "Residence");
                    oleAdapter.TableMappings.Add("Field/Language", "Field/Language");
                    oleAdapter.TableMappings.Add("Prize Name", "Prize Name");
                    oleAdapter.TableMappings.Add("Motivation", "Motivation");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();
                
                //Connect to database in order to transfer data from the DataTable type object,loaded with data from .xlsx fil.
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


