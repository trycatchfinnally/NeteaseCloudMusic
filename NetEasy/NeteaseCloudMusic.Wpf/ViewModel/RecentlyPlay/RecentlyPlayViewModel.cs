using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
   public  class RecentlyPlayViewModel: BindableBase
   {
       private bool _isRecentlyPlayMusicViewModelActived;
       private bool _isRecentlyPlayProgramViewModelActived;
       public RecentlyPlayViewModel()
       {
           IsRecentlyPlayMusicViewModelActived = true;
       }
        public bool IsRecentlyPlayMusicViewModelActived
        {
            get
            {
                return _isRecentlyPlayMusicViewModelActived;
            }

            set
            {
                SetProperty(ref _isRecentlyPlayMusicViewModelActived, value);
                if (value && RecentlyPlayMusicViewModel == null)
                {
                    RecentlyPlayMusicViewModel = CommonServiceLocator.ServiceLocator.Current
                        .GetInstance<RecentlyPlayMusicViewModel>();
                    RaisePropertyChanged(nameof(RecentlyPlayMusicViewModel));
                }
            }
        }

        public bool IsRecentlyPlayProgramViewModelActived
        {
            get
            {
                return _isRecentlyPlayProgramViewModelActived;
            }

            set
            {
                SetProperty(ref _isRecentlyPlayProgramViewModelActived, value);
                if (value && RecentlyPlayProgramViewModel == null)
                {  RecentlyPlayProgramViewModel = CommonServiceLocator.ServiceLocator.Current
                        .GetInstance<RecentlyPlayProgramViewModel>();
                    RaisePropertyChanged(nameof(RecentlyPlayProgramViewModel));
                }
            }
        }

        public RecentlyPlayMusicViewModel RecentlyPlayMusicViewModel { get; private set; }
       public RecentlyPlayProgramViewModel RecentlyPlayProgramViewModel { get; private set; }
    }
}
