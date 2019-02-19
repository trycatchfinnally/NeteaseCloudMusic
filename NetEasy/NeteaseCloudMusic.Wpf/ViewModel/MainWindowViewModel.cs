using NeteaseCloudMusic.Wpf.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Wpf.View;
using NeteaseCloudMusic.Services.AudioDecode;
using NeteaseCloudMusic.Services.NetWork;
using Newtonsoft.Json;
using System.Threading;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _navigationService;
        private readonly IAudioPlayableServices _audioPlayableServices;
        private readonly INetWorkServices _netWorkServices;
        private readonly IEventAggregator _eventAggregator;
        private CancellationTokenSource _timerCancelToken;
        private UserModel _user = new UserModel();
        private ICommand _closePopupCommand;
        private bool _isLike = true;
        private bool _isPlaying;
        private PlayCycleType _cycleType;
        private bool _isExpandCurrentPlayList;
        private Global.Model.Music _currentPlayMusic;
        private int[] _randomIndex;
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
            NavBackCommand = new DelegateCommand(NavBackCommandExecute);
            InitData();
            Context.PlayCommand.RegisterCommand(new DelegateCommand<Global.Model.Music>(Play));
            Context.PauseCommand.RegisterCommand(new DelegateCommand(Pause));
            Context.NextTrackCommand.RegisterCommand(new DelegateCommand(Next));
            Context.PrevTrackCommand.RegisterCommand(new DelegateCommand(Prev));
           
        }

        private void NavBackCommandExecute()
        {
            this._navigationService.Regions[Context.RegionName].NavigationService.Journal.GoBack();

        }

        private void OpenPlayPanelCommandExecute()
        {
            if (CurrentPlayMusic != null)
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.PlayPanelView) +
                    $"?{IndirectView.IndirectViewModelBase.NavigationIdParmmeterName}={CurrentPlayMusic.Id}");
        }
        private void Stop()
        {
            this._audioPlayableServices.Stop();
            this._timerCancelToken?.Cancel();
        }
        /// <summary>
        /// 播放当前的项目
        /// </summary>
        private async void Play(Global.Model.Music music = null)
        {
            Stop();
            if (CurrentPlayList.Count == 0)
            {
                if (music == null) return;
                CurrentPlayList.Add(music);
            }
            if (music == null)
            {
                if (CurrentPlayMusic == null)
                { CurrentPlayMusic = CurrentPlayList[0]; return; }
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
                    return;
                }
            }
            // if (string.IsNullOrEmpty(music.Url))  
            {
                var json = await _netWorkServices.GetAsync("Common", "GetMusicById", new { music.Id });
                var url = JsonConvert.DeserializeObject<Global.Model.Music>(json)?.Url;
                music.Url = url;
                //music.Url = $"https://music.163.com/song/media/outer/url?id={ music.Id}.mp3";
            }
            if (!string.IsNullOrEmpty(music.Url))//如果没有权限或者需要购买等
            {
                _audioPlayableServices.Play(music.Url);
                // IsPlaying = true;
                await TimeRefresherAsync();
            }
            else
            {
                // Context.NextTrackCommand.Execute(null);
            }
            //if (parameters.PlayMusic != null)
            //{
            //    if (parameters.PlayMusic.Id == 0)
            //        throw new ArgumentException("添加的音乐无效！");
            //    var temp = CurrentPlayList.FirstOrDefault(x => x.Id == parameters.PlayMusic.Id);
            //    if (temp != null)
            //    {
            //        CurrentPlayMusic = temp; return;
            //    }
            //    CurrentPlayList.Add(parameters.PlayMusic);
            //    this.CurrentPlayMusic = parameters.PlayMusic; return;
            //}
            //if (_audioPlayableServices.PlayState == PlayState.Playing)
            //{
            //    if (parameters.RePlay)
            //        _audioPlayableServices.Stop();
            //    else
            //    {
            //        _audioPlayableServices.Pause(); IsPlaying = false;
            //        return;
            //    }
            //    //IsPlaying = false;
            //    // return;
            //}
            //else if (_audioPlayableServices.PlayState == PlayState.Paused)
            //{
            //    _audioPlayableServices.Resume(); IsPlaying = true; return;
            //}
            //if (CurrentPlayMusic == null)//给当前音乐赋值的时候会引发当前命令
            //{
            //    CurrentPlayMusic = CurrentPlayList.FirstOrDefault(); return;
            //}
            //var music = CurrentPlayMusic;
            //_audioPlayableServices.Stop();
            //if (music != null)
            //{
            //    if (string.IsNullOrEmpty(music.Url))
            //    {
            //        var json = await _netWorkServices.GetAsync("Common", "GetMusicById", new { music.Id });
            //        var url = JsonConvert.DeserializeObject<Global.Model.Music>(json)?.Url;
            //        music.Url = url;
            //        //music.Url = $"https://music.163.com/song/media/outer/url?id={ music.Id}.mp3";
            //    }

            //    if (!string.IsNullOrEmpty(music.Url))
            //    {
            //        _audioPlayableServices.Play(music.Url);
            //        IsPlaying = true;
            //        await TimeRefresherAsync();
            //    }
            //}
        }
        private void Pause()
        {
            _audioPlayableServices.Pause();

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
                    var indexOf = Array.IndexOf(_randomIndex, currentIndex);
                    if (indexOf == _randomIndex.Length - 1)
                        indexOf = 0;
                    else indexOf++;
                    CurrentPlayMusic = CurrentPlayList[_randomIndex[indexOf]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
                    var indexOf = Array.IndexOf(_randomIndex, currentIndex);
                    if (indexOf == 0)
                        indexOf = _randomIndex.Length - 1;
                    else indexOf--;
                    CurrentPlayMusic = CurrentPlayList[_randomIndex[indexOf]];
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
            _timerCancelToken?.Cancel();
            var newCancel = new CancellationTokenSource();
            _timerCancelToken = newCancel;
            RaisePropertyChanged(nameof(TotalTimeSpan));
            try
            {
                await Task.Factory.StartNew(() =>
                   {
                       while ((TotalTimeSpan - CurrentTimeSpan).TotalMilliseconds > 100)
                       {

                           RaisePropertyChanged(nameof(CurrentTimeSpan));

                           System.Threading.Thread.Sleep(150);
                       }
                   }, this._timerCancelToken.Token);
                Context.NextTrackCommand.Execute(null);
            }
            catch (OperationCanceledException)
            {


            }
            if (_timerCancelToken == newCancel)
                _timerCancelToken = null;
        }


        /// <summary>
        /// 载入数据
        /// </summary>
        private async void InitData()
        {
           // await _netWorkServices.PostAsync("Login", "LoginByCellPhone", new { phone = 15696469238, passWord = "15523738779", remember = true });
            await Task.Delay(100);

        }
        private void SysListCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(SearchView));

                    break;
                case 1:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(FindMusicView));
                    break;
                case 2:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(MvView));
                    break;
                case 3:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(FriendView));
                    break;

            }
        }

        private void MyMusicCommandImpl(int? index)
        {
            switch (index)
            {
                case 0:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(LocalMusicView)); break;
                case 1:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(DownloadManagerView));
                    break;
                case 2:
                    _navigationService.RequestNavigate(Context.RegionName, nameof(RecentlyPlayView));
                    break;
            }
        }



        /// <summary>
        /// 随机播放时，计算随机播放的顺序
        /// </summary>
        private async Task CalcRandomIndex()
        {
            if (_randomIndex == null || _randomIndex.Length != CurrentPlayList.Count)
            {
                _randomIndex = new int[CurrentPlayList.Count];
            }
            if (_randomIndex.All(x => x == 0))
            {
                var tempLst = new System.Collections.Generic.List<int>(_randomIndex.Length);
                for (int i = 0; i < _randomIndex.Length; i++)
                {
                    tempLst.Add(i);
                }

                for (int i = 0; i < _randomIndex.Length; i++)
                {
                    int rint = new Random(DateTime.Now.Millisecond).Next(tempLst.Count);
                    _randomIndex[i] = tempLst[rint];
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


        public UserModel User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }
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
                    _closePopupCommand = new DelegateCommand(() => IsExpandCurrentPlayList = false);
                return _closePopupCommand;
            }
        }

        /// <summary>
        /// 创建的歌单
        /// </summary>
        public ObservableCollection<MenuListItemModel> CreatedTracks { get; } = new ObservableCollection<MenuListItemModel>();
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
                return _audioPlayableServices.Position;
                //return TimeSpan.FromMilliseconds(_audioPlayableServices.Position); 
            }
            set
            {
                _audioPlayableServices.Position = value;
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
            get { return _isLike; }
            set { SetProperty(ref _isLike, value); }
        }
        /// <summary>
        /// 当前的音量
        /// </summary>
        public float Volumn
        {
            get { return _audioPlayableServices.Volumn; }
            set
            {
                _audioPlayableServices.Volumn = value;
                RaisePropertyChanged();
                //LoginRequest.Raise(new Confirmation {Title="4472354" });
            }
        }
        /// <summary>
        /// 当前项目是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {

                if (CurrentPlayList.Count == 0)
                    return;
                SetProperty(ref _isPlaying, value);

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
            get { return _cycleType; }
            set
            {
                SetProperty(ref _cycleType, value);
            }
        }

        /// <summary>
        /// 当前的播放列表
        /// </summary>
        public ObservableCollection<Global.Model.Music> CurrentPlayList
        {
            get;
        } = Context.CurrentPlayMusics;
        /// <summary>
        /// 当前正在播放的项目
        /// </summary>
        public Global.Model.Music CurrentPlayMusic
        {
            get
            {
                return _currentPlayMusic;
            }

            set
            {
                if (value == null)
                {
                    IsPlaying = false;
                    _audioPlayableServices.Stop();
                    //this.TotalTimeSpan = TimeSpan.Zero;
                    //this.CurrentTimeSpan = TimeSpan.Zero;
                }
                else if (CurrentPlayList.Contains(value))
                {
                    SetProperty(ref _currentPlayMusic, value);
                    this.IsPlaying = true;
                    // 
                }
                else throw new ArgumentException("只能播放位于播放列表中的项目，请先添加进当前播放列表！");
                this._eventAggregator.GetEvent<CurrentPlayMusicChangeEventArgs>().Publish(value);
                RaisePropertyChanged(nameof(MusicImage));


                // RaisePropertyChanged(nameof());
            }
        }
        /// <summary>
        /// 是否展开当前播放的项目
        /// </summary>
        public bool IsExpandCurrentPlayList
        {
            get
            {
                return _isExpandCurrentPlayList;
            }

            set { SetProperty(ref _isExpandCurrentPlayList, value); }
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

        public InteractionRequest<Confirmation> LoginRequest { get; } =Context.LoginInteractionRequest;
    }


}
