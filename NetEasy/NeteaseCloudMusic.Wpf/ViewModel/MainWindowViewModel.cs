using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.AudioDecode;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.View;
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

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _navigationService;
        private readonly IAudioPlayableServices _audioPlayableServices;
        private readonly INetWorkServices _netWorkServices;
        private readonly IEventAggregator _eventAggregator;
        private CancellationTokenSource _timerCancelToken;
        private ICommand _closePopupCommand;
        private bool _isPlaying;
        private PlayCycleType _cycleType;
        private bool _isExpandCurrentPlayList;
        private Music _currentPlayMusic;
        private int[] _randomIndex;
        private readonly object _lockObj = new object();
        public MainWindowViewModel(IRegionManager navigationService,
            IAudioPlayableServices audioPlayableServices,
            INetWorkServices netWorkServices,
            IEventAggregator eventAggregator)
        {
            this._navigationService = navigationService;
            this._audioPlayableServices = audioPlayableServices;
            this._netWorkServices = netWorkServices;
            this._eventAggregator = eventAggregator;
            SysListCommand = new DelegateCommand<int?>(SysListCommandImpl);
            MyMusicCommand = new DelegateCommand<int?>(MyMusicCommandImpl);
            OpenPlayPanelCommand = new DelegateCommand(OpenPlayPanelCommandExecute);
            ClearCurrentPlayListCommand = new DelegateCommand(ClearCurrentPlayListCommandImpl);
            UserPlayListCommand = new DelegateCommand<PlayList>(UserPlayListCommandExecute);
            NavBackCommand = new DelegateCommand(NavBackCommandExecute);
            AddPlayListCommand = new DelegateCommand(AddPlayListCommandExecute);
            InitData();
            Context.PlayCommand.RegisterCommand(new DelegateCommand<Music>(Play));
            Context.PauseCommand.RegisterCommand(new DelegateCommand(Pause));
            Context.NextTrackCommand.RegisterCommand(new DelegateCommand(Next));
            Context.PrevTrackCommand.RegisterCommand(new DelegateCommand(Prev));

        }

        private void WriteDebugInfoInText(object msg)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "debug.txt");
            lock (this._lockObj)
            {
                using (var tw = new StreamWriter(filePath, true))
                {
                    tw.WriteLine(DateTime.Now + "      " + msg);
                }
            }
        }
        #region 命令实现
        private void SysListCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(SearchView));

                    break;
                case 1:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(FindMusicView));
                    break;
                case 2:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(MvView));
                    break;
                case 3:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(FriendView));
                    break;

            }
        }

        private void MyMusicCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(LocalMusicView)); break;
                case 1:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(DownloadManagerView));
                    break;
                case 2:
                    this._navigationService.RequestNavigate(Context.RegionName, nameof(RecentlyPlayView));
                    break;
            }
        }
        private void AddPlayListCommandExecute()
        {
            if (Session.IsLoginIn)
                AddPlayListRequest.Raise(new Confirmation { Title = "哈哈哈哈" }, async x => await AddPlayListAsync(x));

        }
        private async Task AddPlayListAsync(Confirmation confirmation)
        {
            if (confirmation.Confirmed)
            {
                string title = confirmation.Content?.ToString();
                if (CreatedTracks.Count(y => y.Name == title) != 0) { return; }//添加提示信息
                var json = await this._netWorkServices.GetAsync("User", "CreatePlayList", new { name = title });
                var playList = JsonConvert.DeserializeObject<PlayList>(json);
                CreatedTracks.Add(playList);
            }
        }
        private void UserPlayListCommandExecute(PlayList obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(IndirectView.IndirectViewModelBase.NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayListDetailView), parmater);
            }
        }

        private void NavBackCommandExecute()
        {
            this._navigationService.Regions[Context.RegionName].NavigationService.Journal.GoBack();

        }

        private void OpenPlayPanelCommandExecute()
        {
            if (CurrentPlayMusic != null)
                this._navigationService.RequestNavigate(Context.RegionName, nameof(PlayPanelView) +
                    $"?{IndirectView.IndirectViewModelBase.NavigationIdParmmeterName}={CurrentPlayMusic.Id}");
        }
        #endregion
        /// <summary>
        /// 表示购买音乐的对应操作流程
        /// </summary>
        /// <param name="music"></param>
        private bool PayMusicProgress(Music music)
        {
            if (music == null) throw new ArgumentNullException(nameof(music));
            bool result = false;
            if (Session.IsLoginIn)
            {
                PayRequest.Raise(new Confirmation { Content = music.Id, Title = "买它狗日的" }, x =>
                  {
                      if (x.Confirmed)
                      {
                          WaitPayRequest.Raise(new Confirmation { Title = "买好了" }, y => result = y.Confirmed);
                      }
                  });
            }
            else
            {
                LoginRequest.Raise(new Confirmation { Title = "登陆" }, x => result = x.Confirmed);

            }
            return result;
        }
        private void Stop()
        {
            this._audioPlayableServices.Stop();
            this._timerCancelToken?.Cancel();
        }
        /// <summary>
        /// 播放当前的项目
        /// </summary>
        private async void Play(Music music = null)
        {
            if (this._audioPlayableServices.PlayState == PlayState.Paused)
            {
                this._audioPlayableServices.Resume();
                WriteDebugInfoInText("play从暂停恢复播放++++++++");
                WriteDebugInfoInText("当前的音乐id" + CurrentPlayMusic?.Id);
                WriteDebugInfoInText("play从暂停恢复播放++++++++");
                return;
            }
            Stop();
            if (CurrentPlayList.Count == 0)
            {
                if (music == null)
                {
                    WriteDebugInfoInText("play传入音乐为null,，播放列表长度为0++++++++");
                    WriteDebugInfoInText("当前的音乐id" + CurrentPlayMusic?.Id);
                    WriteDebugInfoInText("play传入音乐为null，播放列表长度为0++++++++");
                    return;
                }
                CurrentPlayList.Add(music);
            }
            if (music == null)
            {
                if (CurrentPlayMusic == null)
                {
                    CurrentPlayMusic = CurrentPlayList[0];
                    WriteDebugInfoInText("play传入音乐为null,当前音乐为null++++++++");
                    WriteDebugInfoInText("播放列表的第一项idca" + CurrentPlayMusic?.Id);
                    WriteDebugInfoInText("play传入音乐为null当前音乐为null++++++++");
                    return;
                }
                music = CurrentPlayMusic;
            }
            else
            {
                var temp = CurrentPlayList.FirstOrDefault(x => x.Id == music.Id);
                if (temp != null)
                {
                    CurrentPlayMusic = temp;
                }
                else
                {

                    CurrentPlayList.Add(music);
                    CurrentPlayMusic = music;
                  
                }
                WriteDebugInfoInText("play传入音乐在播放列表未找到或者找到，为当前音乐赋值++++++++");
                WriteDebugInfoInText("当前音乐ididca" + CurrentPlayMusic?.Id);
                WriteDebugInfoInText("play传入音乐在播放列表未找到或者找到，为当前音乐赋值++++++++");
                return;

            }
            // if (string.IsNullOrEmpty(music.Url))  
            {
                var json = await this._netWorkServices.GetAsync("Common", "GetMusicById", new { music.Id });
                var url = JsonConvert.DeserializeObject<Music>(json)?.Url;
                music.Url = url;
                WriteDebugInfoInText("play请求url++++++++");
                WriteDebugInfoInText("请求音乐的id" + CurrentPlayMusic?.Id);
                WriteDebugInfoInText("请求音乐地址" + url);
                WriteDebugInfoInText("play请求url，为当前音乐赋值++++++++");
                //music.Url = $"https://music.163.com/song/media/outer/url?id={ music.Id}.mp3";
            }
            if (!string.IsNullOrEmpty(music.Url))//如果没有权限或者需要购买等
            {
                this._audioPlayableServices.Play(music.Url);
                await TimeRefresherAsync();
            }
            else
            {

                if (PayMusicProgress(music))
                {
                    var json = await this._netWorkServices.GetAsync("Common", "GetMusicById", new { music.Id });
                    var url = JsonConvert.DeserializeObject<Music>(json)?.Url;
                    music.Url = url;
                    if (!string.IsNullOrEmpty(music.Url))//如果没有权限或者需要购买等
                    {
                        this._audioPlayableServices.Play(music.Url);
                        // IsPlaying = true;
                        await TimeRefresherAsync();
                    }

                }
            }
        }

        private void Pause()
        {
            this._audioPlayableServices.Pause();
            if (IsPlaying)
            {
                this._isPlaying = false;
                RaisePropertyChanged(nameof(IsPlaying));
            }
        }
        /// <summary>
        /// 下一曲的命令实现
        /// </summary>
        private async void Next()
        {
            if (CurrentPlayList.Count == 0) return;
            var currentIndex = CurrentPlayList.IndexOf(CurrentPlayMusic);
            switch (CycleType)
            {
                case PlayCycleType.RepeatAll:
                    if (currentIndex < CurrentPlayList.Count - 1)
                        CurrentPlayMusic = CurrentPlayList[++currentIndex];
                    else CurrentPlayMusic = CurrentPlayList[0];
                    break;
                case PlayCycleType.RepeatOne:
                    CurrentPlayMusic = CurrentPlayMusic;
                    break;
                case PlayCycleType.Order:
                    if (currentIndex < CurrentPlayList.Count - 1)
                        CurrentPlayMusic = CurrentPlayList[++currentIndex];
                    else CurrentPlayMusic = null;
                    break;
                case PlayCycleType.Random:
                    await CalcRandomIndex();
                    var indexOf = Array.IndexOf(this._randomIndex, currentIndex);
                    if (indexOf == this._randomIndex.Length - 1)
                        indexOf = 0;
                    else indexOf++;
                    CurrentPlayMusic = CurrentPlayList[this._randomIndex[indexOf]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            WriteDebugInfoInText("next++++++++");
            WriteDebugInfoInText("当前的音乐id" + CurrentPlayMusic?.Id);
            WriteDebugInfoInText("next++++++++");


        }
        /// <summary>
        /// 上一个
        /// </summary>
        private async void Prev()
        {
            if (CurrentPlayList.Count == 0) return;
            var currentIndex = CurrentPlayList.IndexOf(CurrentPlayMusic);
            switch (CycleType)
            {
                case PlayCycleType.RepeatAll:
                    if (currentIndex > 0)
                        CurrentPlayMusic = CurrentPlayList[--currentIndex];
                    else CurrentPlayMusic = CurrentPlayList[CurrentPlayList.Count - 1];
                    break;
                case PlayCycleType.RepeatOne:
                    CurrentPlayMusic = CurrentPlayMusic;
                    break;
                case PlayCycleType.Order:
                    if (currentIndex > 0)
                        CurrentPlayMusic = CurrentPlayList[--currentIndex];
                    else CurrentPlayMusic = null;
                    break;
                case PlayCycleType.Random:
                    await CalcRandomIndex();
                    var indexOf = Array.IndexOf(this._randomIndex, currentIndex);
                    if (indexOf == 0)
                        indexOf = this._randomIndex.Length - 1;
                    else indexOf--;
                    CurrentPlayMusic = CurrentPlayList[this._randomIndex[indexOf]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        /// <summary>
        /// 刷新时间
        /// </summary>
        /// <returns></returns>
        private async Task TimeRefresherAsync()
        {
            this._timerCancelToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            this._timerCancelToken = newCancel;
            RaisePropertyChanged(nameof(TotalTimeSpan));
            try
            {
                await Task.Factory.StartNew(() =>
                   {
                       while ((TotalTimeSpan - CurrentTimeSpan).TotalMilliseconds > 100)
                       {

                           RaisePropertyChanged(nameof(CurrentTimeSpan));

                           Thread.Sleep(150);
                       }
                   }, this._timerCancelToken.Token);
                WriteDebugInfoInText("刷新时间的下一曲++++++++");
                WriteDebugInfoInText("当前的音乐id" + CurrentPlayMusic?.Id);
                WriteDebugInfoInText("刷新时间的下一曲++++++++");
                Context.NextTrackCommand.Execute(null);
            }
            catch (OperationCanceledException)
            {


            }
            if (this._timerCancelToken == newCancel)
                this._timerCancelToken = null;
        }


        /// <summary>
        /// 载入数据
        /// </summary>
        private async void InitData()
        {
            Session.LoginStateChanged += Session_LoginStateChanged;
            await Task.Delay(100);

        }

        private async void Session_LoginStateChanged(object sender, bool e)
        {
            CreatedTracks.Clear();
            FavoriteTracks.Clear();
            if (e)
            {
                var json = await this._netWorkServices.GetAsync("User", "GetUserPlayList", new { limit = Context.LimitPerPage, offset = 0, id = Session.CurrentUser.UserId });
                var temp = JsonConvert.DeserializeObject<PlayList[]>(json);
                var query = temp.GroupBy(x => x.CreateUser.UserId == Session.CurrentUser.UserId).ToDictionary(x => x.Key, x => x.Select(y => y));
                CreatedTracks.AddRange(query[true]);
                FavoriteTracks.AddRange(query[false]);
                RaisePropertyChanged(nameof(LoginInUser));
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

        /// <summary>
        /// 随机播放时，计算随机播放的顺序
        /// </summary>
        private async Task CalcRandomIndex()
        {
            if (this._randomIndex == null || this._randomIndex.Length != CurrentPlayList.Count)
            {
                this._randomIndex = new int[CurrentPlayList.Count];
            }
            if (this._randomIndex.All(x => x == 0))
            {
                var tempLst = new System.Collections.Generic.List<int>(this._randomIndex.Length);
                for (int i = 0; i < this._randomIndex.Length; i++)
                {
                    tempLst.Add(i);
                }

                for (int i = 0; i < this._randomIndex.Length; i++)
                {
                    int rint = new Random(DateTime.Now.Millisecond).Next(tempLst.Count);
                    this._randomIndex[i] = tempLst[rint];
                    tempLst.RemoveAt(rint);
                    await Task.Delay(1);
                }
            }
        }

        private void ClearCurrentPlayListCommandImpl()
        {
            CurrentPlayMusic = null;
            CurrentPlayList.Clear();
        }

        private User _defaultUser;
        public User LoginInUser
        {
            get
            {
                if (Session.CurrentUser == null)
                    return this._defaultUser ?? (this._defaultUser = new User());
                return Session.CurrentUser;
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
        #endregion

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
        public string MusicImage
        {
            get { return CurrentPlayMusic?.PicUrl; }
            // set { SetProperty(ref _musicImage, value); }
        }
        /// <summary>
        /// 当前的播放进度
        /// </summary>
        public TimeSpan CurrentTimeSpan
        {
            get
            {
                return this._audioPlayableServices.Position;
                //return TimeSpan.FromMilliseconds(_audioPlayableServices.Position); 
            }
            set
            {
                this._audioPlayableServices.Position = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 当前播放项目总时长
        /// </summary>
        public TimeSpan TotalTimeSpan
        {
            get { return CurrentPlayMusic?.Duration ?? TimeSpan.Zero; }
            // private set { SetProperty(ref _totalTimeSpan, value); }
        }
        /// <summary>
        /// 是否喜欢当前播放的项目
        /// </summary>
        public bool IsLike
        {
            get { return CurrentPlayMusic?.IsLike ?? false; }
            set
            {
                if (CurrentPlayMusic == null || value == CurrentPlayMusic.IsLike) return;
                if (!Session.IsLoginIn)
                {
                    LoginRequest.Raise(new Confirmation { Title = "登陆" });
                    RaisePropertyChanged();
                    return;
                }
                CurrentPlayMusic.IsLike = value;
                LikeOrDisLikeCurrentMusic(value);
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前的音量
        /// </summary>
        public float Volumn
        {
            get { return this._audioPlayableServices.Volumn; }
            set
            {
                this._audioPlayableServices.Volumn = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前项目是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get { return this._isPlaying; }
            set
            {

                if (CurrentPlayList.Count == 0)
                    return;
                SetProperty(ref this._isPlaying, value);

                if (value)
                {
                    Context.PlayCommand.Execute(null);
                }
                else { Context.PauseCommand.Execute(null); }
            }
        }
        /// <summary>
        /// 当前播放的循环模式
        /// </summary>
        public PlayCycleType CycleType
        {
            get { return this._cycleType; }
            set
            {
                SetProperty(ref this._cycleType, value);
            }
        }

        /// <summary>
        /// 当前的播放列表
        /// </summary>
        public ObservableCollection<Music> CurrentPlayList
        {
            get;
        } = Context.CurrentPlayMusics;
        /// <summary>
        /// 当前正在播放的项目
        /// </summary>
        public Music CurrentPlayMusic
        {
            get
            {
                return this._currentPlayMusic;
            }

            set
            {
                if (value == null)
                {
                    IsPlaying = false;
                    this._audioPlayableServices.Stop();
                    //this.TotalTimeSpan = TimeSpan.Zero;
                    //this.CurrentTimeSpan = TimeSpan.Zero;
                }
                else if (CurrentPlayList.Contains(value))
                {
                    SetProperty(ref this._currentPlayMusic, value);
                    IsPlaying = true;
                    // 
                }
                else throw new ArgumentException("只能播放位于播放列表中的项目，请先添加进当前播放列表！");
                this._eventAggregator.GetEvent<CurrentPlayMusicChangeEventArgs>().Publish(value);
                RaisePropertyChanged(nameof(MusicImage));
            }
        }
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
        public InteractionRequest<Confirmation> LoginRequest { get; } = InteractionRequests.LoginInteractionRequest;
        /// <summary>
        /// 添加播放列表弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> AddPlayListRequest { get; } = new InteractionRequest<Confirmation>();
        /// <summary>
        /// 购买音乐弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> PayRequest { get; } = InteractionRequests.PayMusicInteractionRequest;
        /// <summary>
        /// 等待完成支付弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> WaitPayRequest { get; } = new InteractionRequest<Confirmation>();
    }


}
