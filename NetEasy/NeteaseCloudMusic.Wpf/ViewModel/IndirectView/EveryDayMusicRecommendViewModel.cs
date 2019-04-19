using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class EveryDayMusicRecommendViewModel : NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private Music[] selectedMusics;
        private bool _isSelectedModel;
        //是否已经加载好数据，如果已经加载，重新导航到当前页面时不执行联网操作
        private bool _isdataInit;
        public EveryDayMusicRecommendViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            RecomendMvCommand = new DelegateCommand<long?>(RecomendMvCommandExecute);
            PlayAllCommand =new DelegateCommand(PlayAllCommandExecute);
            SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);
            MusicAlbumCommand = new DelegateCommand<Album>(MusicAlbumCommandExecute);
            MusicArtistCommand = new DelegateCommand<long?>(MusicArtistCommandExecute);
        }

        private void MusicArtistCommandExecute(long? id)
        {
            if (id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName,id.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }

        private void MusicAlbumCommandExecute(Album obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }

        private async  void PlayAllCommandExecute()
        {
            Context.CurrentPlayMusics.Clear();
            if (RecommendMusics.Count == 0) return;
            await Context.CurrentPlayMusics.AddRangeAsync(RecommendMusics, range => Context.PlayCommand.Execute(range.First()));
        }
        private async void SelectedCommandExecute(IEnumerable items)
        {
            if (IsSelectedModel)
            {
                this.selectedMusics = items.Cast<Music>().ToArray(); return;
            }
            else if (selectedMusics == null)
            {
                var temp = items.Cast<Music>().FirstOrDefault();
                if (temp != null)
                    Context.PlayCommand.Execute(temp);
                return;
            }

            await Context.CurrentPlayMusics.AddRangeAsync(selectedMusics, source => Context.PlayCommand.Execute(source.First()));
            selectedMusics = null;
        }
        private void RecomendMvCommandExecute(long? mvId)
        {
            if (mvId > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, mvId.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.MvPlayView), parmater);
            }
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!_isdataInit)
            {
                InitData();
                _isdataInit = true;
            }
        }
        private async void InitData()
        {
            var netWorkDataResult=await _netWorkServices.GetAsync<Music[]>("FindMusic", "GetEveryDayMusicRecommend");
            if (netWorkDataResult.Successed)
            {
                var temp = netWorkDataResult.Data;
                await RecommendMusics.AddRangeAsync(temp); 
            }
            else
            {
                //todo 网络失败
            }
        }
        /// <summary>
        /// 每日推荐对应的日
        /// </summary>
        public int Day => DateTime.Now.Day;
        public bool IsSelectedModel
        {
            get { return _isSelectedModel; }
            set { SetProperty(ref _isSelectedModel, value); }
        }
        public SelectionMode SelectionMode => IsSelectedModel ? SelectionMode.Multiple : SelectionMode.Single;
        /// <summary>
        /// 推荐歌曲如果有MV，对应mv的命令
        /// </summary>
        public ICommand RecomendMvCommand { get; }
        public ICommand PlayAllCommand { get; }
        public ICommand SelectedCommand { get; }
        public ICommand MusicAlbumCommand { get; }
        public ICommand MusicArtistCommand { get; }
        public ObservableCollection<Global.Model.Music> RecommendMusics { get; } = new ObservableCollection<Global.Model.Music>();
    }
}
