using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Data.OleDb;
using System.Threading.Tasks;
using Abilitics.Mission.Common;
using Abilitics.Mission.DatabaseInitialization;

namespace Abilitics.Mission
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("Please, specify your .xlsx file path: ");
            // var filePath = Console.ReadLine();

            //var dbName = Console.ReadLine();

            //var commonConfiguration = new CommonConfiguration(dbName);
            DatabaseInitialization();

            ImportToDb(/*filePath*/);
        }

        private static void DatabaseInitialization()
        {
            var ensureDatabaseCreate = new EnsureDatabaseCreated();
            ensureDatabaseCreate.CreateDatabase();

            var ensureTableCreated = new EnsureTableCreated();
            ensureTableCreated.CreateTable();
        }

        private static void ImportToDb(/*string filePath*/)
        {
            var filePath = @"E:\Projects\AbiliticsMission\src\Assignment.Description\Nobel Prize Winners.xlsx";
            //var file = Directory.GetFiles(filePath);

            var connectionString = @"Provider = 'Microsoft.ACE.OLEDB.12.0'; Data Source = '" + filePath + "'; Extended Properties = 'Excel 12.0 Xml;IMEX=1'";

            var excelData = new DataTable();

            using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();

                var sheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();

                var columns = new DataColumn[]
                {
                    new DataColumn("Year",typeof(int)),
                    new DataColumn("Category",typeof(string)),
                    new DataColumn("Name",typeof(string)),
                    new DataColumn("Birthdate",typeof(string)),
                    new DataColumn("Birth Place",typeof(string)),
                    new DataColumn("County",typeof(string)),
                    new DataColumn("Residence",typeof(string)),
                    new DataColumn("Field/Language",typeof(string)),
                    new DataColumn("Prize Name",typeof(string)),
                    new DataColumn("Motivation",typeof(string))
                };

                excelData.Columns.AddRange(columns);

                var cmdText = "SELECT * FROM [" + sheet + "]";

                using (OleDbDataAdapter oleAdapter = new OleDbDataAdapter(cmdText, oleDbConnection))
                {
                    oleAdapter.TableMappings.Add("Year", "Year");
                    oleAdapter.TableMappings.Add("Category", "Category");
                    oleAdapter.TableMappings.Add("Name", "Name");
                    oleAdapter.TableMappings.Add("Birthdate", "Birthdate");
                    oleAdapter.TableMappings.Add("Birth Place", "Birth_Place");
                    oleAdapter.TableMappings.Add("County", "County");
                    oleAdapter.TableMappings.Add("Residence", "Residence");
                    oleAdapter.TableMappings.Add("Field/Language", "Field_Language");
                    oleAdapter.TableMappings.Add("Prize Name", "Prize_Name");
                    oleAdapter.TableMappings.Add("Motivation", "Motivation");

                    oleAdapter.Fill(excelData);

                }

                oleDbConnection.Close();

                using (SqlConnection sqlConnection = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=master;Integrated Security=True"))
                {
                    using (SqlBulkCopy sqlBulk = new SqlBulkCopy(sqlConnection))
                    {
                        sqlBulk.DestinationTableName = "[AbiliticsMission].[dbo].Nobel";


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
                            sqlBulk.WriteToServer(excelData);

                        }
                        catch (Exception ex)
                        {
                            throw new UniqueConstraintViolationException("No new records found for import into database!", ex);
                        }
                        sqlConnection.Close();
                    }
                }
            }
        }
    }
}
