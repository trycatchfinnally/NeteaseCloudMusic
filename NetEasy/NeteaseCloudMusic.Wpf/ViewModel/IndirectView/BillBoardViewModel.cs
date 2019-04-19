using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class BillBoardViewModel : NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;

        /// <summary>
        /// 是否已经加载数据
        /// </summary>
        private bool _dataHasInited;
        public BillBoardViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            BillBoardCommand = new DelegateCommand<Global.Model.BillBoard>(BillBoardCommandExecute);
        }

        private void BillBoardCommandExecute(BillBoard obj)
        {
            if (obj == null) return;
            if (obj.Id != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView), parmater);
            }
            else
            {
                this._navigationService.RequestNavigate(Context.RegionName, nameof(TopArtistsBillBoardView));

            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (this._dataHasInited)
            {
                return;
            }
            var netWorkDataResult= await this._netWorkServices.GetAsync<BillBoard[]>("BillBoard", "GetTopList");
            if (netWorkDataResult.Successed)
            {
                var temp = netWorkDataResult.Data;
                await Task.WhenAll(NeteaseCloudMusicBillBoard.AddRangeAsync(temp.Where(x => (x.SomeTracksName?.Count).GetValueOrDefault() > 0)),
                    GlobalBillBoard.AddRangeAsync(temp.Where(x => x.SomeTracksName == null || x.SomeTracksName.Count == 0)));
                this._dataHasInited = true; 
            }
            else
            {
                //todo 网络连接失败
            }
        }



        /// <summary>
        /// 云音乐榜单
        /// </summary>
        public ObservableCollection<Global.Model.BillBoard> NeteaseCloudMusicBillBoard { get; } = new ObservableCollection<Global.Model.BillBoard>();
        public ObservableCollection<Global.Model.BillBoard> GlobalBillBoard { get; } = new ObservableCollection<Global.Model.BillBoard>();
        public ICommand BillBoardCommand { get; }
    }
}
