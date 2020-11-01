/* LoginRequestData.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


namespace SPTarkov.Launcher
{
	public struct LoginRequestData
	{
		public string email;
		public string password;

		public LoginRequestData(string email, string password)
		{
			this.email = email;
			this.password = password;
		}
	}
}
