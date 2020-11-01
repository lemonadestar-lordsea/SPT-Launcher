/* ClientConfig.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


namespace SPTarkov.Launcher
{
	public class ClientConfig
	{
		public string BackendUrl;
		public string Version;

		public ClientConfig()
		{
			BackendUrl = "https://127.0.0.1";
			Version = "live";
		}

		public ClientConfig(string backendUrl)
		{
			BackendUrl = backendUrl;
			Version = "live";
		}
	}
}
