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
			var filePath = @"D:\PavelS\Playground\AbiliticsMission\src\Assignment.Description";
			//  var file = Directory.GetFiles(filePath);

			var connectionString = @"Provider = 'Microsoft.ACE.OLEDB.12.0'; Data Source = '" + filePath + "'; Extended Properties = 'Excel 12.0 Xml;IMEX=1'";

			var excelData = new DataTable();

			using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
			{
				oleDbConnection.Open();

				//excelData.Columns.AddRange(new DataColumn[] { new DataColumn { "Year",typeof(int),} })

				var sheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
			}
		}
	}
}
