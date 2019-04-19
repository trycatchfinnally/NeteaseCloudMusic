using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class TopArtistsBillBoardViewModel : Prism.Mvvm.BindableBase, Prism.Regions.INavigationAware
    {
        private bool _dataHasInit;
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        public TopArtistsBillBoardViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            SelectedCommand = new DelegateCommand<Global.Model.Artist>(SelectedCommandExecute);
        }

        private void SelectedCommandExecute(Artist obj)
        {
            if (obj == null) return;
            var parmater = new NavigationParameters();
            parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
            this._navigationService.RequestNavigate(Context.RegionName, nameof(ArtistDetailView), parmater);
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

         void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        async void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            if (_dataHasInit)
            {
                return;
            }
            var tasks = new List<Task<NetWorkDataResult<KeyValuePair<DateTime, Global.Model.Artist[]>>>>(4);
            for (int i = 1; i < 5; i++)
            {
                tasks.Add(_netWorkServices.GetAsync<KeyValuePair<DateTime, Global.Model.Artist[]>>("BillBoard", "GetTopArtist", new { type = i }));
            }
            await Task.WhenAll(tasks);
            if (tasks.All(x=>x.Result.Successed))
            {
                var temp = tasks.Select(x => x.Result.Data).ToArray();
                this.LastUpdateDate = temp[0].Key;
                await Task.WhenAll(ChineseArtists.AddRangeAsync(temp[0].Value),
                      EruoArtists.AddRangeAsync(temp[1].Value),
                      KoreaArtists.AddRangeAsync(temp[2].Value),
                      JapanArtists.AddRangeAsync(temp[3].Value));
                RaisePropertyChanged(nameof(LastUpdateDate));
                _dataHasInit = true;

            }else
            {
                //todo 网络连接失败
            }
        }
        /// <summary>
        /// 最近更新
        /// </summary>
        public DateTime LastUpdateDate { get; private set; }
        public ObservableCollection<Global.Model.Artist> ChineseArtists { get; } = new ObservableCollection<Global.Model.Artist>();
        public ObservableCollection<Global.Model.Artist> EruoArtists { get; } = new ObservableCollection<Global.Model.Artist>();
        /// <summary>
        /// 棒子
        /// </summary>
        public ObservableCollection<Global.Model.Artist> KoreaArtists { get; } = new ObservableCollection<Global.Model.Artist>();
        /// <summary>
        /// 鬼子
        /// </summary>
        public ObservableCollection<Global.Model.Artist> JapanArtists { get; } = new ObservableCollection<Global.Model.Artist>();
        public ICommand SelectedCommand { get; }
    }
}
