namespace Aki.Launcher.Attributes
{
    public class RequireLoggedIn : NavigationPreCondition
    {
        public override bool TestPreCondition()
        {
            int status = AccountManager.Login(AccountManager.SelectedAccount.username, AccountManager.SelectedAccount.password);

            return status == 1;
        }
    }
}
