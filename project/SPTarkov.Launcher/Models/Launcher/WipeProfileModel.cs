/* WipeProfileModel.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    public class WipeProfileModel
    {
        public EditionCollection EditionsCollection { get; set; } = new EditionCollection();
    }
}
