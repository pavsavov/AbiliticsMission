using System.Collections.Generic;

namespace Abilitics.Mission.Configurations
{
	/// <summary>
	/// This model is used as DTO for access to the configurations in ApplicationConfig.json file
	/// </summary>
	public class ConfigurationModel
	{
		public Dictionary<string,string> ConnectionStrings { get; set; }

		public Dictionary<string, string> DatabaseConfiguration { get; set; }

		public Dictionary<string, string> ExcelFileConfiguration { get; set; }

	}
}
