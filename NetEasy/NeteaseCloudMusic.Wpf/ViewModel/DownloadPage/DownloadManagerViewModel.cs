using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
 public    class DownloadManagerViewModel: BindableBase
 {
       private bool _isDownloadedMusicViewModelActived ;

     public DownloadManagerViewModel()
     {
            IsDownloadedMusicViewModelActived = true;
     }
        public bool IsDownloadedMusicViewModelActived
        {
            get
            {
                return _isDownloadedMusicViewModelActived;
            }

            set
            {
                SetProperty(ref _isDownloadedMusicViewModelActived, value);
                if (value && DownloadedMusicViewModel == null)
                {
                    DownloadedMusicViewModel =
                        CommonServiceLocator.ServiceLocator.Current.GetInstance<DownloadedMusicViewModel>();
                    RaisePropertyChanged(nameof(DownloadedMusicViewModel));
                }
            }
        }
     public DownloadedMusicViewModel DownloadedMusicViewModel { get; private set; }
    }
}
