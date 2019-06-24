using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Windows.Input;
using NeteaseCloudMusic.Wpf.Properties;
using NeteaseCloudMusic.Wpf.Proxy;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class PlayPanelViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private readonly PlayPartCore _playPart;
        private uint _currentPage = 1;
        private Music _innerMusic = new Music();
        private CommentCollection _innerComment;
        public PlayPanelViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService,

            PlayPartCore playPart
            )
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            _playPart = playPart;
            playPart.MusicChanged += (sender, e) => RefreshMusic(e);
            UserCommand = new DelegateCommand<long?>(UserCommandExecute);
            AlbumCommand = new DelegateCommand<long?>(AlbumCommandExecute);
            ArtistCommand = new DelegateCommand<long?>(ArtistCommandExecute);
            SimiMusicCommand = new DelegateCommand<Music>(SimiMusicCommandExecute);
            SimiPlayListCommand = new DelegateCommand<PlayList>(SimiPlayListCommandExecute);
            AddCommentCommand = new DelegateCommand<string>(AddCommentCommandExecute);
            ThumbsUpCommand = new DelegateCommand<long?>(ThumbsUpCommandExecute);
        }
        /// <summary>
        /// 当当前页发生变化的时候，刷新
        /// </summary>
        private async void OnCurrentPageChanged()
        {
            if (this._innerComment.More)
            {
                var ccDataResult = await this._netWorkServices.GetAsync<CommentCollection>("Common", "GetCommentById", new
                {
                    commentThreadId = $"R_SO_4_{Id}",
                    offset = ((int)CurrentPage-1) * Settings.Default.LimitPerPage,
                    limit = Settings.Default.LimitPerPage
                });
                if (ccDataResult.Successed)
                {
                    this._innerComment = ccDataResult.Data;
                    await Task.WhenAll(NewComments.AddRangeAsync(this._innerComment.Comments),
                        HotComments.AddRangeAsync(this._innerComment.HotComments));
                   // RaiseAllPropertyChanged();
                }
            }
        }
        private async void ThumbsUpCommandExecute(long? obj)
        {
            if (obj > 0)
            {
                await this._netWorkServices.GetAsync<Dictionary<string, object>>("Common", "ThumbsUpComment", new
                {
                    commentId = obj.Value,
                    commentThreadId = Global.Enums.CommentType.R_SO_4_.ToString() + Id,
                    thumbsUp = true
                });
                var query = HotComments.Select((x, i) => new { Model = x, Index = i })
                    .FirstOrDefault(x => x.Model.CommentId == obj);
                if (query != null)
                {
                    query.Model.LikedCount++;
                    HotComments.RemoveAt(query.Index);
                    HotComments.Insert(query.Index, query.Model);
                    return;
                }
                query = NewComments.Select((x, i) => new { Model = x, Index = i })
                    .FirstOrDefault(x => x.Model.CommentId == obj);
                if (query != null)
                {
                    query.Model.LikedCount++;
                    NewComments.RemoveAt(query.Index);
                    NewComments.Insert(query.Index, query.Model);
                }

            }
        }

        private async void AddCommentCommandExecute(string commentContent)
        {
            if (string.IsNullOrEmpty(commentContent) || !Id.HasValue)
            {
                return;
            }
            var netWorkDataResult = await this._netWorkServices.GetAsync<Comment>("Common", "AddComment", new
            {
                resourceId = Id.Value,
                type = Global.Enums.CommentType.R_SO_4_,
                content = commentContent
            });
            if (netWorkDataResult.Successed)
            {
                NewComments.Insert(0, netWorkDataResult.Data);
            }
            else
            {
                //todo 显示提示信息
            }
        }



        /// <summary>
        /// 刷新音乐
        /// </summary>
        /// <param name="music"></param>
        private void RefreshMusic(Music music)
        {
            if (music != null && Id != music.Id)
            {
                Id = music.Id;
                SetById(music.Id);
            }
        }
        private void SimiPlayListCommandExecute(PlayList obj)
        {
            if (obj?.Id > 0)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, obj.Id);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.PlayListDetailView), parmater);
            }
        }
        private async void SimiMusicCommandExecute(Music music)
        {
            if (music != null)
            {
                await this._playPart.Play(music);
            }
        }
        private void UserCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.UserZoneView), parmater);
            }
        }
        private void AlbumCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }
        private void ArtistCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Settings.Default.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }
        protected override async void SetById(long id)
        {
            var task1 = this._netWorkServices.GetAsync<Music>("Music", "GetMusicDetailById", new { id });
            var task2 = this._netWorkServices.GetAsync<List<Music>>("Music", "GetSimiMusic", new { id });
            var task3 = this._netWorkServices.GetAsync<CommentCollection>("Common", "GetCommentById", new { commentThreadId = $"R_SO_4_{id}" });
            var task4 = this._netWorkServices.GetAsync<List<Lyric>>("Music", "GetLyricByMusicId", new { id });
            var task5 = this._netWorkServices.GetAsync<PlayList[]>("Music", "GetSimiPlayList", new { id });
            await Task.WhenAll(task1, task2, task3, task4, task5);

            if (task1.Result.Successed &&
                task2.Result.Successed &&
                task3.Result.Successed &&
                task4.Result.Successed &&
                task5.Result.Successed)
            {
                this._innerMusic = task1.Result.Data;
                this._innerComment = task3.Result.Data;
                await Task.WhenAll(
                Artists.AddRangeAsync(this._innerMusic.Artists),
                SimiMusics.AddRangeAsync(task2.Result.Data),
                NewComments.AddRangeAsync(this._innerComment.Comments),
                HotComments.AddRangeAsync(this._innerComment.HotComments),
                 Lryics.AddRangeAsync(task4.Result.Data),
                 ContainsThisTrackList.AddRangeAsync(task5.Result.Data));
                this._currentPage = 1;
                RaiseAllPropertyChanged();
            }
            else
            {
                //todo 提示信息
            }

        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey(nameof(Id)) && Id > 0)
            {
                var id = System.Convert.ToInt64(navigationContext.Parameters[nameof(Id)]);
                if (id != Id)
                {
                    SetById(Id.Value);
                }
            }


            base.OnNavigatedTo(navigationContext);
        }
        /// <summary>
        /// 代表播放内容的图片
        /// </summary>
        public string TrackImage => this._innerMusic.PicUrl;
        /// <summary>
        /// 代表播放内容的名称
        /// </summary>
        public string TrackName => this._innerMusic.Name;
        /// <summary>
        ///代表对应的专辑名称
        /// </summary>
        public string AlbumName => this._innerMusic.Album?.Name;
        public long? AlbumId => this._innerMusic.Album?.Id;
        /// <summary>
        /// 表示评论的数量
        /// </summary>
        public uint CommentCount
        {
            get
            {
                var temp = this._innerComment?.Total;
                if (temp >= 0)
                {
                    return System.Convert.ToUInt32(temp.Value);
                }

                return 0;
            }
        }

        public uint CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                SetProperty(ref this._currentPage, value);
                OnCurrentPageChanged();
            }
        }

        public ObservableCollection<Artist> Artists { get; } = new ObservableCollection<Artist>();

        /// <summary>
        /// 代表歌词的
        /// </summary>
        public ObservableCollection<Lyric> Lryics { get; } = new ObservableCollection<Lyric>();
        public ObservableCollection<Comment> NewComments { get; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> HotComments { get; } = new ObservableCollection<Comment>();

        #region 命令
        public ICommand UserCommand { get; }
        public ICommand AlbumCommand { get; }
        public ICommand ArtistCommand { get; }
        public ICommand SimiMusicCommand { get; }
        public ICommand SimiPlayListCommand { get; }
        public ICommand AddCommentCommand { get; }
        public ICommand ThumbsUpCommand { get; }
        #endregion

        /// <summary>
        /// 包含这首歌的集合
        /// </summary>
        public ObservableCollection<PlayList> ContainsThisTrackList { get; } = new ObservableCollection<PlayList>();
        /// <summary>
        /// 相似歌曲
        /// </summary>
        public ObservableCollection<Music> SimiMusics { get; } = new ObservableCollection<Music>();
    }
}
