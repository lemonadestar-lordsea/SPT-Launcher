/* RegisterRequestData.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


namespace SPTarkov.Launcher
{
	public struct RegisterRequestData
	{
		public string email;
		public string password;
		public string edition;

		public RegisterRequestData(string email, string password, string edition)
		{
			this.email = email;
			this.password = password;
			this.edition = edition;
		}
	}
}
