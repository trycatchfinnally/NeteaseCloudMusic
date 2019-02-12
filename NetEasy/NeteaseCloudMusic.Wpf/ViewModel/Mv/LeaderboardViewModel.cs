using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Wpf.Model;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// mv排行榜对应的viewmodel
    /// </summary>
   public  class MvLeaderboardViewModel:BindableBase
    {
        private LanguageType _languageType;
        private DateTime _lastUpdateDate=DateTime.Now;

        public MvLeaderboardViewModel()
        {
            for (int i = 0; i < 30; i++)
            {
                LeaderboardMvs.Add(new Model.MvModel {ArtistName = i + "sadghsdfh", Title = "asdgwet"});
            }
            
        }
        public LanguageType LanguageType
        {
            get
            {
                return _languageType;
            }

            set { SetProperty(ref _languageType, value); }
        }
        /// <summary>
        /// 最新更新时间
        /// </summary>
        public DateTime LastUpdateDate
        {
            get
            {
                return _lastUpdateDate;
            }

            set { SetProperty(ref _lastUpdateDate, value); }
        }
        /// <summary>
        /// 对应的mv排行榜
        /// </summary>
        public ObservableCollection<MvModel> LeaderboardMvs { get; } = new ObservableCollection<MvModel>();
    }
}
