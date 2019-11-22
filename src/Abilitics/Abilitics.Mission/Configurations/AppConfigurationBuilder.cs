using Newtonsoft.Json;
using System.IO;

namespace Abilitics.Mission.Configurations
{
	/// <summary>
	/// Reads configuration file then maps the json formated data into a DTO model
	/// </summary>
	public class AppConfigurationBuilder
	{
		private readonly string filePath;

		public AppConfigurationBuilder(string filePath)
		{
			this.filePath = filePath;
		}
		public ConfigurationModel LoadJsonConfigFile()
		{
			var jsonFile = File.ReadAllText(this.filePath);

			ConfigurationModel configModel = JsonConvert.DeserializeObject<ConfigurationModel>(jsonFile);

			return configModel;
		}
	}
}
