using System;
using Aki.Launcher.Models.Aki;

namespace Aki.Launcher.Models.Launcher
{
    public class ProfileInfo
    {
        public string Nickname { get; set; } = "";
        public string SideImage { get; set; } = "";
        public string Side { get; set; } = "";
        public string Level { get; set; } = "";
        public int XPLevelProgress { get; set; } = 0;
        public long CurrentXP { get; set; } = 0;
        public long TillNextLevel { get; set; } = 0;
        public long TotalXP { get; set; } = 0;

        
        public ProfileInfo(ServerProfileInfo serverProfileInfo)
        {
            Nickname = serverProfileInfo.Nickname;
            Side = serverProfileInfo.Side;

            //SideImage = RequestHandler.GetImage(...);

            Level = serverProfileInfo.Level.ToString();
            CurrentXP = serverProfileInfo.CurrExp;
            TotalXP = serverProfileInfo.NextExp;
            TillNextLevel = TotalXP - CurrentXP;

            XPLevelProgress = (int)Math.Floor((((double)TotalXP - serverProfileInfo.PrevExp) - TillNextLevel) / TillNextLevel * 100);

        }
    }
}
