using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilitics.Mission.Common
{
	public class CommonConfiguration
	{
		public CommonConfiguration(string databaseName)
		{
			this.DatabaseName = databaseName;
		}

		public string DatabaseName { get; private set; }

	

	}
}
