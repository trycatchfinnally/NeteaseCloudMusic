using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using Newtonsoft.Json;
using Prism.Regions;
using NeteaseCloudMusic.Wpf.Extensions;
using Prism.Commands;
using System.Collections;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using System.Threading;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class ArtistDetailViewModel : IndirectViewModelBase
    {
        private const int RequireCountPerPage = Context.LimitPerPage;
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private bool _isSelectedModel;
        private Artist _innerArtist = new Artist();
        private Music[] selectedMusics;
        private CancellationTokenSource _albumOffsetCancellationToken;
        private CancellationTokenSource _mvOffsetCancellationToken;

        public ArtistDetailViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            PlayAllCommand = new DelegateCommand(PlayAllCommandExecute);
            SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);
            HotMusicMvCommand = new DelegateCommand<long?>(HotMusicMvCommandExecute);
            AlbumOrMvOffsetCommand = new DelegateCommand<string>(AlbumOrMvOffsetCommandExecute);
        }

        private void AlbumOrMvOffsetCommandExecute(string flag)
        {

            switch (flag)
            {
                case "Album":
                    AlbumOffsetCommandExecute(); return;
                case "Mv":
                    MvOffsetCommandExecute();
                    return;
                default:
                    break;
            }
        }
        private async void AlbumOffsetCommandExecute()
        {
            _albumOffsetCancellationToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            _albumOffsetCancellationToken = newCancel;
            try
            {
                if (MoreAlbums)
                {
                    var netWorkDataResult = await _netWorkServices.GetAsync<KeyValuePair<bool, List<Global.Model.Album>>>("Artist", "GetArtistAlbum",
                        new { id = Id, offset = CurrentAlbumPageOffset * RequireCountPerPage, limit = RequireCountPerPage }, cancelToken: _albumOffsetCancellationToken.Token);
                    if (netWorkDataResult.Successed)
                    {
                        var moreAndAlbums = netWorkDataResult.Data;
                        MoreAlbums = moreAndAlbums.Key;
                        Albums.AddRange(moreAndAlbums.Value);
                        CurrentAlbumPageOffset++;
                    }
                    else
                    {
                        //todo 网络连接失败
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            if (_albumOffsetCancellationToken == newCancel)
                _albumOffsetCancellationToken = null;
        }
        private async void MvOffsetCommandExecute()
        {
            _mvOffsetCancellationToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            _mvOffsetCancellationToken = newCancel;
            try
            {
                if (MoreMvs)
                {
                    var netWorkDataResult= await _netWorkServices.GetAsync<KeyValuePair<bool,Mv[]>>("Artist", "GetArtistMv",
                        new { id = Id, offset = CurrentMvPageOffset, limit = RequireCountPerPage }, cancelToken: _mvOffsetCancellationToken.Token);
                    if (netWorkDataResult.Successed)
                    {
                        var moreAndMvs = netWorkDataResult.Data;
                        MoreMvs = moreAndMvs.Key;
                        Mvs.AddRange(moreAndMvs.Value);
                        CurrentMvPageOffset++;
                    }
                    else
                    {
                        //todo 网络连接失败
                        throw new OperationCanceledException();
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            if (_mvOffsetCancellationToken == newCancel)
                _mvOffsetCancellationToken = null;
        }
        private void HotMusicMvCommandExecute(long? mvId)
        {


            if (mvId.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, mvId.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(MvPlayView), parmater);
            }
        }

        protected override async void SetById(long id)
        {
            var task1 = _netWorkServices.GetAsync<Artist>("Artist", "GetArtistDetailById", new { id });
            var task2 = _netWorkServices.GetAsync<KeyValuePair<bool, List<Global.Model.Album>>>("Artist", "GetArtistAlbum", new { id, offset = 0, limit = RequireCountPerPage });
            var task3 = _netWorkServices.GetAsync<KeyValuePair<bool, List<Global.Model.Mv>>>("Artist", "GetArtistMv", new { id, offset = 0, limit = RequireCountPerPage });
            var task4 = _netWorkServices.GetAsync<List<KeyValuePair<string, string>>>("Artist", "GetArtistIntroduction", new
            {
                id
            });
            var task5 = _netWorkServices.GetAsync<List<Global.Model.Artist>>("Artist", "GetSimiArtists", new { id });
            //  KeyValuePair<bool, List<Global.Model.Album>>
            await Task.WhenAll(task1, task2, task3, task4, task5);
            if (task1.Result.Successed &&
                task2.Result.Successed &&
                task3.Result.Successed &&
                task4.Result.Successed &&
                task5.Result.Successed)
            {
                _innerArtist = task1.Result.Data;
                var moreAndAlbums = task2.Result.Data;
                MoreAlbums = moreAndAlbums.Key;
                var moreAndMvs = task3.Result.Data;
                MoreMvs = moreAndMvs.Key;
                await Task.WhenAll(HotMusics.AddRangeAsync(_innerArtist.HotMusics),
                    Albums.AddRangeAsync(moreAndAlbums.Value),
                    Mvs.AddRangeAsync(moreAndMvs.Value),
                    ArtistIntroductions.AddRangeAsync(task4.Result.Data),
                   SimiArtists.AddRangeAsync(task5.Result.Data));
                RaiseAllPropertyChanged();
            }
            else
            {
                //todo 网络连接四百
            }

        }
        private async void PlayAllCommandExecute()
        {
            Context.CurrentPlayMusics.Clear();
            if (HotMusics.Count == 0) return;
            await Context.CurrentPlayMusics.AddRangeAsync(HotMusics, range => Context.PlayCommand.Execute(range.First()));

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
        /// <summary>
        /// 是否有更多的专辑
        /// </summary>
        private bool MoreAlbums { get; set; }
        /// <summary>
        /// 当前专辑的偏移
        /// </summary>
        private int CurrentAlbumPageOffset { get; set; } = 0;

        /// <summary>
        /// 是否有更多的MV
        /// </summary>
        private bool MoreMvs { get; set; }
        /// <summary>
        /// 当前mv的偏移
        /// </summary>
        private int CurrentMvPageOffset { get; set; } = 0;


        public string PicUrl => _innerArtist?.PicUrl;
        public string AlbumName => _innerArtist?.Name;
        public int MvCount => _innerArtist?.MvCount ?? 0;
        public int MusicCount => _innerArtist?.MusicCount ?? 0;
        public int AlbumCount => _innerArtist?.AlbumCount ?? 0;
        /// <summary>
        /// 热门歌曲选中所有的时候执行的命令
        /// </summary>
        public ICommand PlayAllCommand { get; }
        /// <summary>
        /// 热门歌曲选中执行的命令
        /// </summary>
        public ICommand SelectedCommand { get; }
        /// <summary>
        /// 热门歌曲对应的mv按钮命令
        /// </summary>
        public ICommand HotMusicMvCommand { get; }
        /// <summary>
        /// 选中MV标签MV执行的命令
        /// </summary>
        public ICommand AlbumSelectedCommand { get; }
        /// <summary>
        /// 在有更多的专辑或者MV的时候列表滚动到最后执行的命令
        /// </summary>
        public ICommand AlbumOrMvOffsetCommand { get; }

        public SelectionMode SelectionMode => IsSelectedModel ? SelectionMode.Multiple : SelectionMode.Single;
        /// <summary>
        /// 该歌手的热门
        /// </summary>
        public ObservableCollection<Music> HotMusics { get; } = new ObservableCollection<Music>();
        public ObservableCollection<Album> Albums { get; } = new ObservableCollection<Album>();
        public ObservableCollection<Mv> Mvs { get; } = new ObservableCollection<Mv>();
        /// <summary>
        /// 歌手介绍的信息
        /// </summary>
        public ObservableCollection<KeyValuePair<string, string>> ArtistIntroductions { get; } = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<Artist> SimiArtists { get; } = new ObservableCollection<Artist>();
        public bool IsSelectedModel
        {
            get { return this._isSelectedModel; }
            set { SetProperty(ref _isSelectedModel, value); }
        }
    }
}
