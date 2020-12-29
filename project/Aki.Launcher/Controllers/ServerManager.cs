/* ServerManager.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.Windows;
using Aki.Launcher.MiniCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aki.Launcher
{
    public static class ServerManager
	{
		public static List<ServerInfo> AvailableServers { get; private set; } = new List<ServerInfo>();
		public static ServerInfo SelectedServer { get; private set; } = null;

		public static void SelectServer(int index)
		{
			if (index < 0 || index >= AvailableServers.Count)
			{
				SelectedServer = null;
				return;
			}

			SelectedServer = AvailableServers[index];
		}

		public static void LoadServer(string backendUrl)
		{
			string json = "";

			try
			{
				RequestHandler.ChangeBackendUrl(backendUrl);
				json = RequestHandler.RequestConnect();
			}
			catch
			{
				return;
			}

			AvailableServers.Add(Json.Deserialize<ServerInfo>(json));
		}

		public static async Task LoadDefaultServerAsync(string server)
        {
			AvailableServers.Clear();

			await Task.Run(() =>
			{
				LoadServer(server);
			});
        }

		public static void LoadServers(string[] servers)
		{
			AvailableServers.Clear();

			foreach (string backendUrl in servers)
			{
				LoadServer(backendUrl);
			}
		}
	}
}
