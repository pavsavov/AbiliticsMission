using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace Abilitics.Mission
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Please, specify your .xlsx file path: ");

            // var filePath = Console.ReadLine();

            ImportToDb(/*filePath*/);
        }

        private static void ImportToDb(/*string filePath*/)
        {
            var filePath = @"E:\Projects\AbiliticsMission\src\Assignment.Description\";
            //  var file = Directory.GetFiles(filePath);

            var connectionString = @"Provider = 'Microsoft.ACE.OLEDB.12.0'; Data Source = '" + filePath + "'; Extended Properties = 'Excel 12.0 Xml;IMEX=1'";

            CreateDatabase();

            CreateTable();

            //using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            //{
            //    oleDbConnection.CreateCommand
            //}
        }

        private static void CreateDatabase()
        {
            var myConnectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Integrated Security=True";

            using (var connection = new SqlConnection(myConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "CREATE DATABASE AbiliticsMission";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Should use Async
        /// </summary>
        /// <returns></returns>
        private static void CreateTable()
        {
            var myConnectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Integrated Security=True";

            using (var connection = new SqlConnection(myConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"CREATE TABLE dbo.Nobel
                (
                    Id int IDENTITY(1,1) NOT NULL,
                    Year DateTime NOT NULL,
                    Category NVARCHAR(255) NOT NULL,
                    Name NVARCHAR(255) NOT NULL,
                    Birthdate DATETIME NULL,
                    Birth_Place NVARCHAR(255) NULL,
                    Country NVARCHAR(255) NULL,
                    Residence NVARCHAR(255) NULL,
                    Field_Language NVARCHAR(255) NULL,
                    Prize_Name NVARCHAR(255) NOT NULL,
                    Motivation NVARCHAR(255) NOT NULL, 
                    CONSTRAINT pk_id PRIMARY KEY (Id)
                );";

                command.ExecuteNonQuery();
            }
        }
    }
}
