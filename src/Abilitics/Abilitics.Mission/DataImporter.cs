using Abilitics.Mission.Common;
using Abilitics.Mission.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission
{
    public class DataImporter
    {
        private readonly ConfigurationModel configurations;
        private readonly SqlQueryContainer sqlQueryContainer;

        public DataImporter(ConfigurationModel configurations, SqlQueryContainer sqlQueryContainer)
        {
            this.configurations = configurations;
            this.sqlQueryContainer = sqlQueryContainer;
        }

        /// <summary>
        /// Logical connection between data load from excel file and data import to database.
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        public void DatabaseRecordsImport()
        {
            var loadedDataFromExcelSheet = this.MapAndLoadFromExcelFile();

            this.ImportToDatabase(loadedDataFromExcelSheet);
        }

        /// <summary>
        /// Read and load data from .xlsx file into DataTable collection
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        private DataTable MapAndLoadFromExcelFile()
        {
            var excelData = new DataTable();
            var oleDbConnectionString = this.configurations.ExcelFileConfiguration["FileConnection"];

            using (OleDbConnection oleDbConnection = new OleDbConnection(oleDbConnectionString))
            {
                oleDbConnection.Open();

                var sheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();


                var columns = new DataColumn[]
                {
                    new DataColumn($"{this.sqlQueryContainer.Year}",typeof(int)),
                    new DataColumn($"{this.sqlQueryContainer.Category}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.Name}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.Birthdate}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.BirthPlace}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.County}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.Residence}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.FieldLanguage}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.PrizeName}",typeof(string)),
                    new DataColumn($"{this.sqlQueryContainer.Motivation}",typeof(string))
                };

                excelData.Columns.AddRange(columns);

                var commandText = this.sqlQueryContainer.SelectExcelFileSheet(sheet);

                //Map and Fill DataTable type object with records from excel table. 
                using (OleDbDataAdapter oleAdapter = new OleDbDataAdapter(commandText, oleDbConnection))
                {
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Year}", $"{this.sqlQueryContainer.Year}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Category}", $"{this.sqlQueryContainer.Category}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Name}", $"{this.sqlQueryContainer.Name}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Birthdate}", $"{this.sqlQueryContainer.Birthdate}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.BirthPlace}", $"{this.sqlQueryContainer.BirthPlace}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.County}", $"{this.sqlQueryContainer.County}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Residence}", $"{this.sqlQueryContainer.Residence}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.FieldLanguage}", $"{this.sqlQueryContainer.FieldLanguage}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.PrizeName}", $"{this.sqlQueryContainer.PrizeName}");
                    oleAdapter.TableMappings.Add($"{this.sqlQueryContainer.Motivation}", $"{this.sqlQueryContainer.Motivation}");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();

                DataView view = new DataView(excelData);
                DataTable distinctExcelRows = view.ToTable(true,
                    $"{this.sqlQueryContainer.Year}", $"{this.sqlQueryContainer.Category}", $"{this.sqlQueryContainer.Name}", $"{this.sqlQueryContainer.Birthdate}", $"{this.sqlQueryContainer.BirthPlace}", $"{this.sqlQueryContainer.County}", $"{this.sqlQueryContainer.Residence}", $"{this.sqlQueryContainer.FieldLanguage}", $"{this.sqlQueryContainer.PrizeName}", $"{this.sqlQueryContainer.Motivation}");

                return distinctExcelRows;
            }
        }

        /// <summary>
        /// Import loaded data from DataTable type collection into database table.
        /// </summary>
        /// <param name="configurations">DTO for access to .json configuration file.</param>
        /// <param name="sqlQueryContainer">Holds Application SQL queries and database  operations related strings</param>
        /// <param name="excelData">Mapped and loaded DataTable data structure with records from Excel sheet</param>
        private void ImportToDatabase(DataTable excelData)
        {
            // Connect to database in order to transfer data from the DataTable type object, loaded with data from.xlsx fil.
            using (SqlConnection sqlConnection = new SqlConnection(this.configurations.ConnectionStrings["ApplicationConnection"]))
            {
                var queryAllDataInDb = this.sqlQueryContainer.GetDataFromDbTable();
                var connectionString = this.configurations.ConnectionStrings["ApplicationConnection"];
                var currentDataInDb = PullData(connectionString, queryAllDataInDb);

                var rows = currentDataInDb.Rows.Count > 0;

                if (rows)
                {
                    var truncateTableQuery = this.sqlQueryContainer.TruncateTable();

                    using (SqlCommand command = new SqlCommand(truncateTableQuery, sqlConnection))
                    {
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }

                // define INSERT query with parameters
                string query = this.sqlQueryContainer.InsertRecordsInTable();

                //count excel file rows;
                int rowCounter = 1;

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    foreach (var row in excelData.AsEnumerable())
                    {
                        rowCounter++;

                        if (rowCounter == 514)
                        {
                            Console.WriteLine("514");
                            Console.ReadKey();
                        }

                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Year)}", SqlDbType.Int).Value = row.ItemArray[0] is DBNull ? (object)DBNull.Value : (int)row.ItemArray[0];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Category)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[1] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[1];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Name)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[2] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[2];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Birthdate)}", SqlDbType.DateTime).Value = row.ItemArray[3] is DBNull ? (object)DBNull.Value : DateFormatter(row.ItemArray[3], rowCounter);
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.BirthPlace)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[4] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[4];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.County)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[5] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[5];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Residence)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[6] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[6];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.FieldLanguage)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[7] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[7];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.PrizeName)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[8] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[8];
                        command.Parameters.Add($"@{nameof(this.sqlQueryContainer.Motivation)}", SqlDbType.NVarChar, 255).Value = row.ItemArray[9] is DBNull ? (object)DBNull.Value : (string)row.ItemArray[9];

                        sqlConnection.Close();
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                        sqlConnection.Close();

                        command.Parameters.Clear();

                    }
                }
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

                var excelValueAsDateTimeTemp = Convert.ToDateTime(excelDateValue).Year;
                var formatedDateTime = Convert.ToDateTime(excelDateValue).ToString("dd-MMM-y");

                var getYearAsFourDigits = culture.Calendar.ToFourDigitYear(excelValueAsDateTimeTemp);

                if (DateTime.TryParseExact(formatedDateTime, new string[2] { "dd MMMM YYYY", "dd-MMM-y" }, culture, DateTimeStyles.None, out dateTime))
                {
                    var resultNewDate = new DateTime(getYearAsFourDigits, dateTime.Month, dateTime.Day);

                    return resultNewDate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import process was interupted! Invalid data format in column 'Birthdate' at row {rowCounter}. " +
                                  $"Please provide row value in valid date-time format: e.g.'DD-MMM-YY'\r\n\r\nException:\r\n\r\n");
                throw;
            }
            return dateTime;

        }

        #endregion

    }
}
