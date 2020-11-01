/* AccountInfo.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


namespace SPTarkov.Launcher
{
	public class AccountInfo
	{
		public string id;
		public string nickname;
		public string email;
		public string password;
		public bool wipe;
		public string edition;

		public AccountInfo()
		{
			id = "";
			nickname = "";
			email = "";
			password = "";
			wipe = false;
			edition = "";
		}
	}
}
