/* IHavePassword.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * waffle.lord
 */

namespace SPTarkov.Launcher.Interfaces
{
    //This is not meant to be secure, it is only so we can use a passwordbox to hide passwords for streamers and such.
    public interface IHavePassword
    {
        string Password { get; }
    }
}
