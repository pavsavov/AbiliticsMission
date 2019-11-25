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
        /// Set your path to the ApplicationConfig.json configuration file as shown in the example.
        /// </summary>
        private const string configFilePath = @"E:\Projects\AbiliticsMission\src\Abilitics\Abilitics.Mission\ApplicationConfig.json";

        static void Main(string[] args)
        {
            //Load application configurations from ApplicationConfig.json file
            var configBuilder = new AppConfigurationBuilder(configFilePath);
            var configurations = configBuilder.LoadJsonConfigFile();
            var sqlQueryContainer = new SqlQueryContainer(configurations);

            //Configurations and SqlQueryContainer validation
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

            var dateImporter = new DataImporter(configurations, sqlQueryContainer);
            var appOutput = new ApplicationOutput();

            //Import process
            try
            {
                dateImporter.DatabaseRecordsImport();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.WriteLine(appOutput.Output());
        }
    }
}


