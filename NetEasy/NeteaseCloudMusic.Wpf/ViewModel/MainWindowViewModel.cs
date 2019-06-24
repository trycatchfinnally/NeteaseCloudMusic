using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.AudioDecode;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.View;
using NeteaseCloudMusic.Wpf.View.Cloud;
using NeteaseCloudMusic.Wpf.View.IndirectView;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using NeteaseCloudMusic.Services.Identity;
using NeteaseCloudMusic.Wpf.Properties;
using NeteaseCloudMusic.Wpf.Proxy;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _navigationService;
        private readonly INetWorkServices _netWorkServices;
        private readonly IdentityService _dentityService;
        private readonly PlayPartCore _playPart;
        private readonly InteractionRequestsProxy _interactionRequestsProxy;
        private ICommand _closePopupCommand;
        private PlayCycleType _cycleType;
        private bool _isExpandCurrentPlayList;
        private bool _timeFlag;
        public MainWindowViewModel(IRegionManager navigationService,
            INetWorkServices netWorkServices,
            IdentityService dentityService,
            PlayPartCore playPart,
            InteractionRequestsProxy interactionRequestsProxy
            )
        {
            this._navigationService = navigationService;
            this._netWorkServices = netWorkServices;
            this._dentityService = dentityService;
            this._playPart = playPart;
            this._interactionRequestsProxy = interactionRequestsProxy;
            this._playPart.MusicChanged += _playPart_MusicChanged;
            this._playPart.PlayStateChanged += _playPart_PlayStateChanged;
            SysListCommand = new DelegateCommand<int?>(SysListCommandImpl);
            MyMusicCommand = new DelegateCommand<int?>(MyMusicCommandImpl);
            OpenPlayPanelCommand = new DelegateCommand(OpenPlayPanelCommandExecute);
            ClearCurrentPlayListCommand = new DelegateCommand(ClearCurrentPlayListCommandImpl);
            UserPlayListCommand = new DelegateCommand<PlayList>(UserPlayListCommandExecute);
            NavBackCommand = new DelegateCommand(NavBackCommandExecute);
            AddPlayListCommand = new DelegateCommand(AddPlayListCommandExecute);
            NextCommand = new DelegateCommand(NextCommandExecute);
            PrevCommand = new DelegateCommand(PrevCommandExecute);
            // InitData();
            dentityService.LoginStateChanged += Session_LoginStateChanged;
            Session_LoginStateChanged(null, dentityService.CurrentUser != null);

        }

        private void _playPart_MusicChanged(object sender, Music e)
        {
            RaisePropertyChanged(nameof(CurrentPlayMusic));
        }

        private void _playPart_PlayStateChanged(object sender, PlayState e)
        {
            if (!this._timeFlag)
            {
                this._timeFlag = true;
                TimeRefresher();
            }
        }
        #region 命令实现
        private void SysListCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(SearchView));

                    break;
                case 1:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(FindMusicView));
                    break;
                case 2:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(MvView));
                    break;
                case 3:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(FriendView));
                    break;

            }
        }

        private void MyMusicCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(LocalMusicView)); break;
                case 1:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(DownloadManagerView));
                    break;
                case 2:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(RecentlyPlayView)); break;
                case 3:
                    this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(CloudMusicView));
                    break;
            }
        }
        private void AddPlayListCommandExecute()
        {
            if (this._dentityService.CurrentUser != null)
                AddPlayListRequest.Raise(new Confirmation { Title = "哈哈哈哈" }, async x => await AddPlayListAsync(x));

        }

        private void NextCommandExecute() => this._playPart.Next();

        private void PrevCommandExecute() => this._playPart.Prev();
        private async Task AddPlayListAsync(Confirmation confirmation)
        {
            if (confirmation.Confirmed)
            {
                string title = confirmation.Content?.ToString();
                if (CreatedTracks.Count(y => y.Name == title) != 0) { return; }//添加提示信息
                var netWorkDataResult = await this._netWorkServices.GetAsync<PlayList>("User", "CreatePlayList", new { name = title });
                if (netWorkDataResult.Successed)
                {
                    var playList = netWorkDataResult.Data;
                    CreatedTracks.Add(playList);
                }
                else
                {
                    //  Context.AutoDisplayPopupRequest.Raise(new Notification { Content = "网络不给力额，请检查你的网络连接~", Title = "我和" });

                }
            }
        }
        private void UserPlayListCommandExecute(PlayList obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(PlayListDetailView), parmater);
            }
        }

        private void NavBackCommandExecute()
        {
            this._navigationService.Regions[Settings.Default.RegionName].NavigationService.Journal.GoBack();

        }

        private void OpenPlayPanelCommandExecute()
        {
            if (this._playPart.CurrentMusic != null)
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(PlayPanelView) +
                    $"?{IndirectView.IndirectViewModelBase.NavigationIdParmmeterName}={this._playPart.CurrentMusic.Id}");
        }
        #endregion



        /// <summary>
        /// 刷新时间
        /// </summary>
        /// <returns></returns>
        private void TimeRefresher()
        {
            //这里下一个会涉及跨线程问题，所以这里需要使用委托
            var dispatcher = System.Windows.Application.Current.MainWindow.Dispatcher;
            ThreadPool.QueueUserWorkItem(x =>
            {
                while (true)
                {
                    RaisePropertyChanged(nameof(DurationMilliseconds));
                    RaisePropertyChanged(nameof(PositionMilliseconds));
                    RaisePropertyChanged(nameof(IsPlaying));
                    if (Math.Abs(DurationMilliseconds) > 1e-6 && DurationMilliseconds - PositionMilliseconds <= 100.0)
                        dispatcher.Invoke(this._playPart.Next);
                    Thread.Sleep(150);
                }
            });
            //this._timerCancelToken?.Cancel();
            //var newCancel = new CancellationTokenSource();
            //this._timerCancelToken = newCancel;
            //RaisePropertyChanged(nameof(TotalTimeSpan));
            //try
            //{
            //    var thePlayFlag = false;
            //    thePlayFlag = await Task.Factory.StartNew(() =>
            //        {
            //            while (true)
            //            {
            //                if (TotalTimeSpan == TimeSpan.Zero)
            //                    continue;
            //                if ((TotalTimeSpan - CurrentTimeSpan).TotalMilliseconds > 100)
            //                    RaisePropertyChanged(nameof(CurrentTimeSpan));
            //                else return true;
            //                Thread.Sleep(150);
            //            }
            //        }, this._timerCancelToken.Token);
            //    WriteDebugInfoInText("刷新时间的下一曲++++++++");
            //    WriteDebugInfoInText("当前的音乐id" + this._playPart.CurrentMusic?.Id);
            //    WriteDebugInfoInText("刷新时间的下一曲++++++++");
            //    if (thePlayFlag)
            //        this._commandsProxy.NextTrackCommand.Execute(null);
            //}
            //catch (OperationCanceledException)
            //{


            //}
            //if (this._timerCancelToken == newCancel)
            //    this._timerCancelToken = null;
        }


        /// <summary>
        /// 载入数据
        /// </summary>
        //private async void InitData()
        //{
        //    await Task.Delay(100);

        //}

        private async void Session_LoginStateChanged(object sender, bool e)
        {
            CreatedTracks.Clear();
            FavoriteTracks.Clear();
            if (e)
            {
                var netWorkDataResult = await this._netWorkServices.GetAsync<PlayList[]>("User", "GetUserPlayList", new { limit = Settings.Default.LimitPerPage, offset = 0, id = this._dentityService.CurrentUser.UserId });
                if (netWorkDataResult.Successed)
                {
                    var temp = netWorkDataResult.Data;
                    var query = temp.GroupBy(x => x.CreateUser.UserId == this._dentityService.CurrentUser.UserId).ToDictionary(x => x.Key, x => x.Select(y => y));
                    CreatedTracks.AddRange(query[true]);
                    FavoriteTracks.AddRange(query[false]);
                    RaisePropertyChanged(nameof(LoginInUser));
                }
                else
                {
                    //  Context.AutoDisplayPopupRequest.Raise(new Notification { Content = "网络不给力额，请检查你的网络连接~", Title = "我和" });

                }
            }

        }


        /// <summary>
        /// 喜欢某音乐或者不喜欢
        /// </summary>
        /// <param name="like"></param>
        private async void LikeOrDisLikeCurrentMusic(bool like)
        {
            if (like)
                await Task.Delay(1);
        }

        private void ClearCurrentPlayListCommandImpl()
        {
            //CurrentPlayMusic = null;
            this._playPart.MusicsListCollection.Clear();
        }

        private User _defaultUser;
        public User LoginInUser
        {
            get
            {
                if (this._dentityService.CurrentUser == null)
                    return this._defaultUser ?? (this._defaultUser = new User());
                return this._dentityService.CurrentUser;
            }
        }
        #region 绑定的命令
        /// <summary>
        /// 发现音乐等部分对应的命令
        /// </summary>
        public ICommand SysListCommand { get; }

        /// <summary>
        /// 清除当前播放列表的命令
        /// </summary>
        public ICommand ClearCurrentPlayListCommand { get; }

        /// <summary>
        /// 关闭弹出的播放列表部分
        /// </summary>
        public ICommand ClosePopupCommand
        {
            get
            {
                if (this._closePopupCommand == null)
                    this._closePopupCommand = new DelegateCommand(() => IsExpandCurrentPlayList = false);
                return this._closePopupCommand;
            }
        }
        /// <summary>
        /// 我的音乐部分对应的命令
        /// </summary>
        public ICommand MyMusicCommand { get; }

        public ICommand OpenPlayPanelCommand { get; }
        /// <summary>
        /// 导航后退的命令
        /// </summary>
        public ICommand NavBackCommand { get; }
        public ICommand UserPlayListCommand { get; }
        /// <summary>
        /// 添加播放列表的命令
        /// </summary>
        public ICommand AddPlayListCommand { get; }
        /// <summary>
        /// 下一曲对应的命令
        /// </summary>
        public ICommand NextCommand { get; }
        /// <summary>
        /// 上一曲对应的命令
        /// </summary>
        public ICommand PrevCommand { get; }

        #endregion
        /// <summary>
        /// 获取总的时长，以毫秒表示
        /// </summary>
        public Double TotalMillSeconds => this._playPart.TotalTimeSpan.TotalMilliseconds;
        /// <summary>
        /// 获取或设置总的时长，以毫秒表示，用于绑定到UI的进度条
        /// </summary>
        public double PositionMillSeconds
        {
            get { return this._playPart.PositionTimeSpan.TotalMilliseconds; }

            set
            {
                this._playPart.PositionTimeSpan = TimeSpan.FromMilliseconds(value);
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 创建的歌单
        /// </summary>
        public ObservableCollection<PlayList> CreatedTracks { get; } = new ObservableCollection<PlayList>();
        /// <summary>
        /// 收藏的歌单
        /// </summary>
        public ObservableCollection<PlayList> FavoriteTracks { get; } = new ObservableCollection<PlayList>();

        /// <summary>
        /// 当前播放音乐的图片
        /// </summary>
        public string MusicImage => this._playPart.CurrentMusic?.PicUrl;

        /// <summary>
        /// 当前的播放进度
        /// </summary>
        public double PositionMilliseconds
        {
            get
            {
                return this._playPart.PositionTimeSpan.TotalMilliseconds;
                //return TimeSpan.FromMilliseconds(_audioPlayableServices.Position); 
            }
            set
            {
                this._playPart.PositionTimeSpan = TimeSpan.FromMilliseconds(value);
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 当前播放项目总时长
        /// </summary>
        public double DurationMilliseconds => this._playPart.TotalTimeSpan.TotalMilliseconds;

        /// <summary>
        /// 是否喜欢当前播放的项目
        /// </summary>
        public bool IsLike
        {
            get { return this._playPart.CurrentMusic?.IsLike ?? false; }
            set
            {
                var temp = this._playPart.CurrentMusic;
                if (temp == null || value == temp.IsLike) return;
                if (this._dentityService.CurrentUser == null)
                {
                    LoginRequest.Raise(new Confirmation { Title = "登陆" });
                    RaisePropertyChanged();
                    return;
                }
                temp.IsLike = value;
                LikeOrDisLikeCurrentMusic(value);
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前的音量
        /// </summary>
        public float Volumn
        {
            get { return this._playPart.Volumn; }
            set
            {
                this._playPart.Volumn = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前项目是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            //get { return this._isPlaying; }
            //set
            //{

            //    if (CurrentPlayList.Count == 0)
            //        return;
            //    SetProperty(ref this._isPlaying, value);

            //    if (value)
            //    {
            //        this._commandsProxy.PlayCommand.Execute(null);
            //    }
            //    else { this._commandsProxy.PauseCommand.Execute(null); }
            //}
            get { return this._playPart.IsPlaying; }
            set
            {
                if (this._playPart.MusicsListCollection.Count == 0)
                {
                    return;

                }
                if (value)
                {
                    this._playPart.Play().Wait(300);
                }
                else
                {
                    this._playPart.Pause();
                }
            }
        }
        /// <summary>
        /// 当前播放的循环模式
        /// </summary>
        public PlayCycleType CycleType
        {
            get { return this._playPart.CurrentCycleType; }
            set
            {
                this._playPart.CurrentCycleType = value;
                RaisePropertyChanged();

            }
        }

        /// <summary>
        /// 当前的播放列表
       // / </summary>
        public ObservableCollection<Music> CurrentPlayList
        {
            get { return this._playPart.MusicsListCollection; }
        }

        /// <summary>
        /// 当前正在播放的项目
        /// </summary>
        public Music CurrentPlayMusic
        {
            get
            {
                return this._playPart.CurrentMusic;
            }
            set
            {
                this._playPart.CurrentMusic = value; RaisePropertyChanged();
            if(value!=null)
                    _playPart.Play(value).Wait(1000);
            }
        }
        //{}
        //    get
        //    {
        //        return this._currentPlayMusic;
        //    }

        //    set
        //    {
        //        if (value == null)
        //        {
        //            IsPlaying = false;
        //            this._audioPlayableServices.Stop();
        //            //this.TotalTimeSpan = TimeSpan.Zero;
        //            //this.CurrentTimeSpan = TimeSpan.Zero;
        //        }
        //        else if (CurrentPlayList.Contains(value))
        //        {
        //            SetProperty(ref this._currentPlayMusic, value);
        //            IsPlaying = true;
        //            // 
        //        }
        //        else throw new ArgumentException("只能播放位于播放列表中的项目，请先添加进当前播放列表！");
        //        this._eventAggregator.GetEvent<CurrentPlayMusicChangeEventArgs>().Publish(value);
        //        RaisePropertyChanged(nameof(MusicImage));
        //    }
        //}
        /// <summary>
        /// 是否展开当前播放的项目
        /// </summary>
        public bool IsExpandCurrentPlayList
        {
            get
            {
                return this._isExpandCurrentPlayList;
            }

            set { SetProperty(ref this._isExpandCurrentPlayList, value); }
        }

        /// <summary>
        /// 登陆弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> LoginRequest => this._interactionRequestsProxy.LoginInteractionRequest;
        /// <summary>
        /// 添加播放列表弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> AddPlayListRequest { get; } = new InteractionRequest<Confirmation>();

        /// <summary>
        /// 购买音乐弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> PayRequest => this._interactionRequestsProxy.PayMusicInteractionRequest;

        /// <summary>
        /// 等待完成支付弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> WaitPayRequest => this._interactionRequestsProxy.WaitPayRequest;
        /// <summary>
        /// 用来弹出播放模式
        /// </summary>
        public InteractionRequest<Notification> AutodisappeaRequest =>
            this._interactionRequestsProxy.AutoDisappearPopupRequest;
    }


}
