/* ChangeRequestData.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


namespace SPTarkov.Launcher
{
	public struct ChangeRequestData
	{
		public string email;
		public string password;
		public string change;

		public ChangeRequestData(string email, string password, string change)
		{
			this.email = email;
			this.password = password;
			this.change = change;
		}
	}
}
