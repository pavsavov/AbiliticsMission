using Abilitics.Mission.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.DatabaseInitialization
{
    public class EnsureDatabaseCreated
    {
        public void CreateDatabase()
        {
            var myConnectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Integrated Security=True";
            var cmdText = "CREATE DATABASE AbiliticsMission";
            var dbName = "AbiliticsMission";
            var dbExists = CheckDatabaseExists(dbName,myConnectionString);


            if (!dbExists)
            {
                using (var sqlConnection = new SqlConnection(myConnectionString))
                {
                    using (var sqlCommand = new SqlCommand(cmdText, sqlConnection))
                    {
                        sqlConnection.Open();
                        var rowsAffected = sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        /// <summary>
        /// probably wil be reworked
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private bool CheckDatabaseExists(string dataBase,string connectionString)
        {
            string cmdText = "SELECT * FROM master.dbo.sysdatabases WHERE name ='" + dataBase + "'";
            bool isExist = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }
                con.Close();
            }
            return isExist;
        }
    }
}
