using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class PlayPanelViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;

        private Global.Model.Music _innerMusic = new Global.Model.Music();
        private Global.Model.CommentCollection _innerComment;
        public PlayPanelViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService, IEventAggregator eventAggregator
            )
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            eventAggregator.GetEvent<CurrentPlayMusicChangeEventArgs>().Subscribe(RefreshMusic);
            UserCommand = new DelegateCommand<long?>(UserCommandExecute);
            AlbumCommand = new DelegateCommand<long?>(AlbumCommandExecute);
            ArtistCommand = new DelegateCommand<long?>(ArtistCommandExecute);
            SimiMusicCommand = new DelegateCommand<Music>(SimiMusicCommandExecute);
            SimiPlayListCommand = new DelegateCommand<PlayList>(SimiPlayListCommandExecute);
            AddCommentCommand = new DelegateCommand<string>(AddCommentCommandExecute);
        }

        private async void AddCommentCommandExecute(string commentContent)
        {
            if (string.IsNullOrEmpty(commentContent))
            {
                return;
            }
            var json = await this._netWorkServices.GetAsync("Common", "AddComment", new
            {
                resourceId = Id.Value,
                type = Global.Enums.CommentType.R_SO_4_,
                content = commentContent
            });
            NewComments.Insert(0, JsonConvert.DeserializeObject<Global.Model.Comment>(json));
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
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.PlayListDetailView), parmater);
            }
        }
        private void SimiMusicCommandExecute(Music music)
        {
            if (music != null)
            {
                Context.PlayCommand.Execute(music);
            }
        }
        private void UserCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.UserZoneView), parmater);
            }
        }
        private void AlbumCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.AlbumView), parmater);
            }
        }
        private void ArtistCommandExecute(long? id)
        {
            if (id.HasValue)
            {
                var parmater = new NavigationParameters();
                parmater.Add(NavigationIdParmmeterName, id.Value);
                this._navigationService.RequestNavigate(Context.RegionName, nameof(View.IndirectView.ArtistDetailView), parmater);
            }
        }
        protected override async void SetById(long id)
        {
            var task1 = this._netWorkServices.GetAsync("Music", "GetMusicDetailById", new { id });
            var task2 = this._netWorkServices.GetAsync("Music", "GetSimiMusic", new { id });
            var task3 = this._netWorkServices.GetAsync("Common", "GetCommentById", new { commentThreadId = $"R_SO_4_{id}" });
            var task4 = this._netWorkServices.GetAsync("Music", "GetLyricByMusicId", new { id });
            var task5 = this._netWorkServices.GetAsync("Music", "GetSimiPlayList", new { id });
            await Task.WhenAll(task1, task2, task3, task4, task5);
            this._innerMusic = JsonConvert.DeserializeObject<Music>(task1.Result);
            this._innerComment = JsonConvert.DeserializeObject<CommentCollection>(task3.Result);
            await Task.WhenAll(
            Artists.AddRangeAsync(this._innerMusic.Artists),
            SimiMusics.AddRangeAsync(JsonConvert.DeserializeObject<List<Music>>(task2.Result)),
            NewComments.AddRangeAsync(this._innerComment.Comments),
            HotComments.AddRangeAsync(this._innerComment.HotComments),
             Lryics.AddRangeAsync(JsonConvert.DeserializeObject<List<Lyric>>(task4.Result)),
             ContainsThisTrackList.AddRangeAsync(JsonConvert.DeserializeObject<PlayList[]>(task5.Result)));
            RaiseAllPropertyChanged();

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
        public ObservableCollection<Artist> Artists { get; } = new ObservableCollection<Artist>();
        public int CommentCount => this._innerComment?.Total ?? 0;
        /// <summary>
        /// 代表歌词的
        /// </summary>
        public ObservableCollection<Global.Model.Lyric> Lryics { get; } = new ObservableCollection<Lyric>();
        public ObservableCollection<Comment> NewComments { get; } = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> HotComments { get; } = new ObservableCollection<Comment>();

        public ICommand UserCommand { get; }
        public ICommand AlbumCommand { get; }
        public ICommand ArtistCommand { get; }
        public ICommand SimiMusicCommand { get; }
        public ICommand SimiPlayListCommand { get; }
        public ICommand AddCommentCommand { get; }

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
