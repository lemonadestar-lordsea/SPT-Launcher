using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Aki.Launcher.MiniCommon;
using Aki.Launcher.Models.Aki;

namespace Aki.Launcher.Models.Launcher
{
    public class ProfileInfo : INotifyPropertyChanged
    {
        private string _Nickname;
        public string Nickname
        {
            get => _Nickname;
            set
            {
                if(_Nickname != value)
                {
                    _Nickname = value;
                    RaisePropertyChanged(nameof(Nickname));
                }
            }
        }

        private string _SideImage;
        public string SideImage
        {
            get => _SideImage;
            set
            {
                if(_SideImage != value)
                {
                    _SideImage = value;
                    RaisePropertyChanged(nameof(SideImage));
                }
            }
        }

        private string _Side;
        public string Side
        {
            get => _Side;
            set
            {
                if(_Side != value)
                {
                    _Side = value;
                    RaisePropertyChanged(nameof(Side));
                }
            }
        }

        private string _Level;
        public string Level
        {
            get => _Level;
            set
            {
                if(_Level != value)
                {
                    _Level = value;
                    RaisePropertyChanged(nameof(Level));
                }
            }
        }

        private int _XPLevelProgress;
        public int XPLevelProgress
        {
            get => _XPLevelProgress;
            set
            {
                if(_XPLevelProgress != value)
                {
                    _XPLevelProgress = value;
                    RaisePropertyChanged(nameof(XPLevelProgress));
                }
            }
        }

        private long _CurrentXP;
        public long CurrentXP
        {
            get => _CurrentXP;
            set
            {
                if (_CurrentXP != value)
                {
                    _CurrentXP = value;
                    RaisePropertyChanged(nameof(CurrentXP));
                }
            }
        }

        private long _TillNextLevel;
        public long TillNextLevel
        {
            get => _TillNextLevel;
            set
            {
                if(_TillNextLevel != value)
                {
                    _TillNextLevel = value;
                    RaisePropertyChanged(nameof(TillNextLevel));
                }
            }
        }

        private long _TotalXP;
        public long TotalXP
        {
            get => _TotalXP;
            set
            {
                if(_TotalXP != value)
                {
                    _TotalXP = value;
                    RaisePropertyChanged(nameof(TotalXP));
                }
            }
        }

        private bool _HasData;
        public bool HasData
        {
            get => _HasData;
            set
            {
                if(_HasData != value)
                {
                    _HasData = value;
                    RaisePropertyChanged(nameof(HasData));
                }
            }
        }

        public void UpdateDisplayedProfile(ProfileInfo PInfo)
        {
            if (PInfo.Side == null || string.IsNullOrWhiteSpace(PInfo.Side) || PInfo.Side == "unknown") return;

            HasData = true;
            Nickname = PInfo.Nickname;
            Side = PInfo.Side;

            //invalidate the image (incase it needs to be reloaded)
            RaisePropertyChanged(nameof(SideImage));

            Level = PInfo.Level;
            CurrentXP = PInfo.CurrentXP;
            TotalXP = PInfo.TotalXP;
            TillNextLevel = PInfo.TillNextLevel;
            XPLevelProgress = PInfo.XPLevelProgress;
        }
        
        public ProfileInfo(ServerProfileInfo serverProfileInfo)
        {
            Nickname = serverProfileInfo.Nickname;
            Side = serverProfileInfo.Side;

            SideImage = Path.Combine(ImageRequest.ImageCacheFolder, $"side.png");

            if(Side != null && !string.IsNullOrWhiteSpace(Side) && Side != "unknown")
            {
                HasData = true;
            }
            else
            {
                HasData = false;
            }

            Level = serverProfileInfo.Level.ToString();
            CurrentXP = serverProfileInfo.CurrExp;
            TotalXP = serverProfileInfo.NextExp;
            TillNextLevel = TotalXP - CurrentXP;

            XPLevelProgress = (int)Math.Floor((((double)TotalXP - serverProfileInfo.PrevExp) - TillNextLevel) / TillNextLevel * 100);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
