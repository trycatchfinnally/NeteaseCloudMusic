using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.AudioDecode;
using NeteaseCloudMusic.Services.Identity;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Proxy;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;

namespace NeteaseCloudMusic.Wpf
{
    public class PlayPartCore
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IAudioPlayableServices _audioPlayableServices;
        private readonly ILoggerFacade _logger;
        private readonly IdentityService _dentityService;
        private readonly InteractionRequestsProxy _interactionRequestsProxy;
        private Music _currentMusic;
        private PlayTypes _playType = PlayTypes.Online;
        private PlayCycleType _currentCycleType = PlayCycleType.Order;
        private int[] _randomIndex;
        /// <summary>
        /// 当播放状态发生变化时，例如从暂停变为播放
        /// </summary>
        public event EventHandler<PlayState> PlayStateChanged;

        public event EventHandler PlayTypesChanged;
        /// <summary>
        /// 当音乐变化的时候。例如下一曲
        /// </summary>
        public event EventHandler<Music> MusicChanged;
        public PlayPartCore(INetWorkServices netWorkServices,
            IAudioPlayableServices audioPlayableServices,
             ILoggerFacade logger,
            IdentityService dentityService,
            InteractionRequestsProxy interactionRequestsProxy)
        {
            this._netWorkServices = netWorkServices;
            this._audioPlayableServices = audioPlayableServices;
            this._logger = logger;
            _dentityService = dentityService;
            this._interactionRequestsProxy = interactionRequestsProxy;

        }

        /// <summary>
        /// 生成随机的序列
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private int[] RandomSequence(int length)
        {
            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = i;
            }


            for (int i = length - 1; i > 0; i--)
            {
                var tmp = new Random().Next(i);
                System.Threading.Thread.Sleep(5);
                result[i] = result[i] + result[tmp];
                result[tmp] = result[i] - result[tmp];
                result[i] = result[i] - result[tmp];
            }

            return result;

        }
        /// <summary>
        /// 代表当前的循环模式
        /// </summary>
        public PlayCycleType CurrentCycleType
        {
            get { return this._currentCycleType; }
            set
            {
                if (value == this.CurrentCycleType)
                {
                    return;
                }

                if (OnPlayCycleTypeChanging(this._currentCycleType, value))
                {
                    this._currentCycleType = value;
                    Notification notification = new Notification { Title = "是根深蒂固" };
                    switch (value)
                    {
                        case PlayCycleType.RepeatAll:
                            notification.Content = "列表循环";
                            break;
                        case PlayCycleType.RepeatOne:
                            notification.Content = "单曲循环";
                            break;
                        case PlayCycleType.Order:
                            notification.Content = "顺序播放";
                            break;
                        case PlayCycleType.Random:
                            notification.Content = "随机播放";
                            break;
                        default:
                            break;
                    }
                    this._interactionRequestsProxy.AutoDisappearPopupRequest.Raise(notification);
                    if (value == PlayCycleType.Random &&
                        (this._randomIndex == null || this._randomIndex.Length != MusicsListCollection.Count || this._randomIndex.All(x => x == 0)))
                    {
                        this._randomIndex = RandomSequence(MusicsListCollection.Count);
                    }
                }



            }
        }
        /// <summary>
        /// 表示当前播放的类型
        /// </summary>
        public PlayTypes PlayType
        {
            get { return _playType; }
            set
            {
                if (value==this._playType)
                {
                    return;
                }
                this._playType = value; 
                PlayTypesChanged?.Invoke(this,EventArgs.Empty);
            }
        }
        /// <summary>
        /// 代表当前的播放列表
        /// </summary>
        public ObservableCollection<Music> MusicsListCollection { get; } = new ObservableCollection<Music>();
        /// <summary>
        /// 表示当前的项目
        /// </summary>
        public Music CurrentMusic
        {
            get { return this._currentMusic; }
            set
            {
                if (value == null)
                {
                    this._audioPlayableServices.Stop();
                    this._currentMusic = null;
                    return;
                }
                else
                {
                    if (!MusicsListCollection.Contains(value))
                        MusicsListCollection.Add(value);
                    this._currentMusic = value;
                }
                //this._eventAggregator.GetEvent<CurrentPlayMusicChangeEventArgs>().Publish(value);
                MusicChanged?.Invoke(this, value);
            }
        }
        /// <summary>
        /// 音量
        /// </summary>
        public Single Volumn
        {
            get { return this._audioPlayableServices.Volumn; }
            set { this._audioPlayableServices.Volumn = value; }
        }
        /// <summary>
        /// 总的时长
        /// </summary>
        public TimeSpan TotalTimeSpan
        {

            get
            {
                var result = CurrentMusic?.Duration;
                if (result.HasValue && result > TimeSpan.Zero)
                {
                    return result.Value;
                }

                return this._audioPlayableServices?.Length ?? TimeSpan.Zero;
            }
        }
        /// <summary>
        /// 代表当前的播放进度，可用来控制播放进度
        /// </summary>
        public TimeSpan PositionTimeSpan
        {
            get { return this._audioPlayableServices.Position; }
            set
            {
                if (value > TotalTimeSpan) return;
                this._audioPlayableServices.Position = value;
            }
        }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying => this._audioPlayableServices.PlayState == PlayState.Playing;
        /// <summary>
        /// 表示购买音乐的对应操作流程
        /// </summary>
        /// <param name="music"></param>
        private bool PayMusicProgress(Music music)
        {
            if (music == null) throw new ArgumentNullException(nameof(music));
            bool result = false;
            if (this._dentityService.CurrentUser != null)
            {
                this._interactionRequestsProxy.PayMusicInteractionRequest.Raise(new Confirmation { Content = music.Id, Title = "买它狗日的" }, x =>
                {
                    if (x.Confirmed)
                    {
                        this._interactionRequestsProxy.WaitPayRequest.Raise(new Confirmation { Title = "买好了" }, y => result = y.Confirmed);
                    }
                });
            }
            else
            {
                this._interactionRequestsProxy.LoginInteractionRequest.Raise(new Confirmation { Title = "登陆" }, x => result = x.Confirmed);

            }
            return result;
        }
        /// <summary>
        /// 当循环模式即将变化是需要执行的逻辑
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual bool OnPlayCycleTypeChanging(PlayCycleType oldValue, PlayCycleType newValue)
        {

            return true;
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            this._audioPlayableServices.Pause();
            PlayStateChanged?.Invoke(this, this._audioPlayableServices.PlayState);
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            this._audioPlayableServices.Stop();
            this._audioPlayableServices.Position = TimeSpan.Zero;
            PlayStateChanged?.Invoke(this, this._audioPlayableServices.PlayState);
        }
        /// <summary>
        /// 播放指定的音乐
        /// </summary>
        /// <param name="music">如果传入null，则自动播放当前列表的第一个</param>
        public async Task Play(Music music = null)
        {
            if (music == null && MusicsListCollection.Count == 0)
            {
                throw new ArgumentException();
            }

            if (this._audioPlayableServices.PlayState == PlayState.Paused)
            {
                this._audioPlayableServices.Resume();
                return;
            }
            Stop();
            if (MusicsListCollection.Count == 0)
            {
                MusicsListCollection.Add(music);
                CurrentMusic = music;
            }

            if (music == null)
            {
                CurrentMusic = MusicsListCollection[0];
            }
            else
            {
                var tmp = MusicsListCollection.FirstOrDefault(x => x.Id == music.Id);
                if (tmp == null)
                {
                    MusicsListCollection.Add(music);
                    CurrentMusic = music;
                }
                else
                {
                    CurrentMusic = tmp;
                }
            }
            var netWorkDataResult = await this._netWorkServices.GetAsync<Music>("Common", "GetMusicById", new { CurrentMusic.Id });
            if (!netWorkDataResult.Successed)
            {
                this._logger.Log($"请求音乐地址失败，对应的id：{CurrentMusic.Id},名称{CurrentMusic.Name}", Category.Exception, Priority.High);
                return;
            }

            CurrentMusic.Url = netWorkDataResult.Data.Url;
            if (!string.IsNullOrEmpty(netWorkDataResult.Data.Url))
            {
                this._audioPlayableServices.Play(netWorkDataResult.Data.Url);
                PlayStateChanged?.Invoke(this, this._audioPlayableServices.PlayState);
            }
            else if (PayMusicProgress(CurrentMusic))
            {
                netWorkDataResult = await this._netWorkServices.GetAsync<Music>("Common", "GetMusicById", new { CurrentMusic.Id });
                if (!netWorkDataResult.Successed)
                {
                    this._logger.Log($"请求音乐地址失败，对应的id：{CurrentMusic.Id},名称{CurrentMusic.Name}", Category.Exception, Priority.High);
                    return;
                }
                CurrentMusic.Url = netWorkDataResult.Data.Url;
                if (!string.IsNullOrEmpty(netWorkDataResult.Data.Url))
                {
                    this._audioPlayableServices.Play(netWorkDataResult.Data.Url);
                    PlayStateChanged?.Invoke(this, this._audioPlayableServices.PlayState);
                }
            }
        }

        /// <summary>
        /// 下一曲，根据当前的播放模式确定下一曲的内容
        /// </summary>
        public void Next()
        {
            if (MusicsListCollection.Count == 0)
            {
                return;
            }

            var currentIndex = MusicsListCollection.IndexOf(CurrentMusic);
            switch (CurrentCycleType)
            {
                case PlayCycleType.RepeatAll:
                    if (currentIndex < MusicsListCollection.Count - 1)
                    {
                        CurrentMusic = MusicsListCollection[currentIndex + 1];
                    }
                    else CurrentMusic = MusicsListCollection[0];
                    break;
                case PlayCycleType.RepeatOne:
                    CurrentMusic = CurrentMusic;
                    break;
                case PlayCycleType.Order:
                    if (currentIndex < MusicsListCollection.Count - 1)
                        CurrentMusic = MusicsListCollection[currentIndex + 1];
                    else CurrentMusic = null;
                    break;
                case PlayCycleType.Random:
                    if (this._randomIndex == null || this._randomIndex.Length != MusicsListCollection.Count || this._randomIndex.All(x => x == 0))
                    {
                        this._randomIndex = RandomSequence(MusicsListCollection.Count);
                    }
                    var randomIndex = this._randomIndex.IndexOf(currentIndex);
                    if (randomIndex == this._randomIndex.Length - 1)
                        randomIndex = 0;
                    else randomIndex++;
                    CurrentMusic = MusicsListCollection[this._randomIndex[randomIndex]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (CurrentMusic != null)
            {
                Play(CurrentMusic).Wait(1000);
            }
            else Stop();

        }

        public void Prev()
        {
            if (MusicsListCollection.Count == 0)
            {
                return;
            }

            var currentIndex = MusicsListCollection.IndexOf(CurrentMusic);
            switch (CurrentCycleType)
            {
                case PlayCycleType.RepeatAll:
                    if (currentIndex == 0)
                    {
                        CurrentMusic = MusicsListCollection[MusicsListCollection.Count - 1];
                    }
                    else CurrentMusic = MusicsListCollection[currentIndex - 1];
                    break;
                case PlayCycleType.RepeatOne:
                    CurrentMusic = CurrentMusic;
                    break;
                case PlayCycleType.Order:
                    if (currentIndex > 0)
                        CurrentMusic = MusicsListCollection[currentIndex - 1];
                    else CurrentMusic = null;
                    break;
                case PlayCycleType.Random:
                    if (this._randomIndex == null || this._randomIndex.Length != MusicsListCollection.Count || this._randomIndex.All(x => x == 0))
                    {
                        this._randomIndex = RandomSequence(MusicsListCollection.Count);
                    }
                    var randomIndex = this._randomIndex.IndexOf(currentIndex);
                    if (randomIndex == 0)
                        randomIndex = this._randomIndex.Length - 1;
                    else randomIndex--;
                    CurrentMusic = MusicsListCollection[this._randomIndex[randomIndex]];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (CurrentMusic != null)
            {
                Play(CurrentMusic).Wait(1000);
            }
            else Stop();
        }
    }
}
