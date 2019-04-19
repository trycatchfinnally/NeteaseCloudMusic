using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 最新音乐tab页面对应的viewmodel
    /// </summary>
    public class NewMusicViewModel : BindableBase
    {

        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private bool _isSelectModel;
        private Global.Model.Music[] selectedMusics;
        private int _pageOffset = 0;
        private int _totalSize;
        private CancellationTokenSource _offsetCancellationToken;

        public NewMusicViewModel(INetWorkServices netWorkServices, IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            PlayAllOrSelectedCommand = new DelegateCommand(PlayAllOrSelectedCommandImpl);
            NewMusicOrDiskCommand = new DelegateCommand<string>(NewMusicOrDiskCommandImpl);
            LanguageCommand = new DelegateCommand<string>(GetTopMusic);
            SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandImpl);
            ArtistCommand = new DelegateCommand<Global.Model.Artist>(ArtistCommandExecute);
            AlbumCommand = new DelegateCommand<long?>(AlbumCommandExecute);
            NextPageCommand = new DelegateCommand(async () => { await TopAlbumAsync(); });
            SelectedAlbumCommand = new DelegateCommand<Album>(SelectedAlbumCommandExecute);
            GetTopMusic("1");
        }

        private void SelectedAlbumCommandExecute(Album obj)
        {
            if (obj != null && obj.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }

        private void AlbumCommandExecute(long? obj)
        {
            if (obj.HasValue && obj.Value > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Value);
                //  this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }

        private void ArtistCommandExecute(Artist obj)
        {
            if (obj != null && obj.Id != 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(ArtistDetailView), parmater);
            }
        }

        private async void GetTopMusic(string type)
        {
            NewMusicList.Clear();
            var temp = string.IsNullOrEmpty(type) ? 1 : int.Parse(type);
            var netWorkDataResult= await this._netWorkServices.GetAsync< Music[]>("FindMusic", "TopMusics", new { Type = temp });
            if (netWorkDataResult.Successed)
            {
                await NewMusicList.AddRangeAsync(netWorkDataResult.Data);
            }
            else
            {
                //todo 网络提示
            }
            //await NewMusicList.AddRangeAsync(JsonConvert.DeserializeObject<Global.Model.Music[]>(json));
        }
        private async void NewMusicOrDiskCommandImpl(string msg)
        {
            if (msg == "0" && NewMusicList.Count == 0)//新歌速递
            {

            }
            else if (msg == "1" && NewAlbumList.Count == 0)//新碟
            {
                this._pageOffset = 0;
                this._totalSize = 1;
                await TopAlbumAsync();
            }
        }
        private async Task TopAlbumAsync()
        {
            this._offsetCancellationToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            this._offsetCancellationToken = newCancel;
            try
            {
                if (this._totalSize <= this._pageOffset * Context.LimitPerPage) return;
                var netWorkDataResult= await this._netWorkServices.GetAsync< KeyValuePair<int, Album[]>>("FindMusic", "TopAlbum",
                    new
                    {
                        limit = Context.LimitPerPage,
                        offset = this._pageOffset
                    });
                if (netWorkDataResult.Successed)
                {
                    var temp = netWorkDataResult.Data;
                    this._totalSize = temp.Key;
                    NewAlbumList.AddRange(temp.Value);
                    this._pageOffset++;
                }
              //  var temp = JsonConvert.DeserializeObject<KeyValuePair<int, Album[]>>(json);
                
            }
            catch (OperationCanceledException)
            {


            }
            if (this._offsetCancellationToken == newCancel)
                this._offsetCancellationToken = null;
        }
        private async void PlayAllOrSelectedCommandImpl()
        {
            if (!IsSelectModel && NewMusicList.Count > 0)
            {

                await Context.CurrentPlayMusics.AddRangeAsync(NewMusicList, x => Context.PlayCommand.Execute(x.First()));

            }


        }



        /// <summary>
        /// 选择完成后需要执行的方法
        /// </summary>
        /// <param name="items"></param>
        private async void SelectedCommandImpl(IEnumerable items)
        {
            if (items == null) return;
            var temp = items.Cast<Global.Model.Music>().ToArray();
            if (IsSelectModel)
            {
                this.selectedMusics = temp;
                return;
            }
            else if (temp.Length > 0)
            {
                Context.PlayCommand.Execute(temp[0]);
                return;
            }
            if (this.selectedMusics == null)
                return;
            //_mainWindowViewModel.CurrentPlayList.Clear();
            //_mainWindowViewModel.CurrentPlayList.AddRange(selectedMusics);
            //_mainWindowViewModel.PlayFirstOrDefault();
            await Context.CurrentPlayMusics.AddRangeAsync(this.selectedMusics, x => Context.PlayCommand.Execute(x.First()));
            this.selectedMusics = null;

        }
        /// <summary>
        /// 最新音乐列表
        /// </summary>
        public ObservableCollection<Global.Model.Music> NewMusicList
        {
            get
            ;

        } = new ObservableCollection<Global.Model.Music>();
        public ObservableCollection<Album> NewAlbumList { get; } = new ObservableCollection<Album>();
        /// <summary>
        /// 获取或设置当前页面是用户选择还是播放全部
        /// </summary>
        public bool IsSelectModel
        {
            get
            {
                return this._isSelectModel;
            }

            set
            {
                if (value == this._isSelectModel) return;
                SetProperty(ref this._isSelectModel, value);

                RaisePropertyChanged(nameof(SelectionMode));


            }
        }
        /// <summary>
        /// 用来绑定到页面是否是选择状态以避免额外的值转换器
        /// </summary>
        public SelectionMode SelectionMode => this._isSelectModel ? SelectionMode.Extended : SelectionMode.Single;
        /// <summary>
        /// 播放全部或者选中的音乐
        /// </summary>
        public ICommand PlayAllOrSelectedCommand { get; }
        /// <summary>
        /// 在选择的时候执行的命令
        /// </summary>
        public ICommand SelectedCommand { get; }
        /// <summary>
        /// 新歌或者新碟对应的命令
        /// </summary>
        public ICommand NewMusicOrDiskCommand { get; }

        /// <summary>
        /// 语言对应的命令
        /// </summary>
        public ICommand LanguageCommand { get; }
        /// <summary>
        /// 点击项目对应的歌手的时候触发的事件
        /// </summary>
        public ICommand ArtistCommand { get; }
        /// <summary>
        /// 点击项目对应的专辑执行的命令
        /// </summary>
        public ICommand AlbumCommand { get; }
        /// <summary>
        /// 专辑向下滑动的时候
        /// </summary>
        public ICommand NextPageCommand { get; }
        /// <summary>
        /// 新碟上架部分选中执行的命令
        /// </summary>
        public ICommand SelectedAlbumCommand { get; }
    }
}
