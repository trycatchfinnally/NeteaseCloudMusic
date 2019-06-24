using NeteaseCloudMusic.Wpf.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Windows.Input;
using NeteaseCloudMusic.Global.Enums;
using Prism.Commands;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Properties;
using Prism.Regions;
using Newtonsoft.Json;
using NeteaseCloudMusic.Wpf.View.IndirectView;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 精选页面对应的viewmodel
    /// </summary>
   public  class FeaturedViewModel:NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        public FeaturedViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            MvCommand = new DelegateCommand<Mv>(MvCommandExecute);


        }

        private void MvCommandExecute(Mv obj)
        {
            if (obj?.Id>0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(MvPlayView), parmater);
            }
        }

        /// <summary>
        /// 点击更多时候执行的命令
        /// </summary>
        /// <param name="type"></param>
        private void MoreCommandImpl(string  type)
        {
            switch (type)
            {
                case "1":
                    break;
                case "2":
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
        
        /// <summary>
        /// 网易出品
        /// </summary>
        public ObservableCollection<Global.Model.Mv> NeteaseProduceds { get; } = new ObservableCollection<Mv>();
        /// <summary>
        /// 最新mv
        /// </summary>
        public ObservableCollection<Mv> NewMvs { get; } = new ObservableCollection<Mv>();
        /// <summary>
        /// 更多对应的命令
        /// </summary>
        public ICommand MoreCommand { get; }
        public ICommand MvCommand { get; }
    }
}
