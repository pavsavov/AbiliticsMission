using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.Common
{
	public class SqlCommands
	{
		public static string CreateDbTable { get; private set; } = @"CREATE TABLE dbo.Nobel
				(
					Id int IDENTITY(1,1) NOT NULL,
					Year int NOT NULL,
                    Category NVARCHAR(255) NOT NULL,
					Name NVARCHAR(255) NOT NULL,
					Birthdate DATETIME NULL,
					Birth_Place NVARCHAR(255) NULL,
                    Country NVARCHAR(255) NULL,
                    Residence NVARCHAR(255) NULL,
                    Field_Language NVARCHAR(255) NULL,
                    Prize_Name NVARCHAR(255) NOT NULL,
					Motivation NVARCHAR(255) NOT NULL,
					CONSTRAINT pk_id PRIMARY KEY(Id)
                );";

		//Will be aded dynamicly a name
		public static string CreateDatabase { get; private set; } = $"SELECT database_id FROM sys.databases WHERE Name = AbiliticsMission";

	}
}
