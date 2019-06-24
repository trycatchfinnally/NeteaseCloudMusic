using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Prism.Commands;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class AlbumViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private readonly PlayPartCore _playPart;
        private Global.Model.Album _innerAlbum = new Global.Model.Album();
        private bool _isSelectedModel;
        private Music[] _selectedMusics;
        public AlbumViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService, PlayPartCore playPart)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            this._playPart = playPart;
            PlayAllCommand = new DelegateCommand(PlayAllCommandExecute);
            this.SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);

        }
        protected override async void SetById(long id)
        {
            var netWorkDataResult = await this._netWorkServices.GetAsync<Album>("Album", "GetAlbumDetailById", new { id });
            if (netWorkDataResult.Successed)
            {
                _innerAlbum = netWorkDataResult.Data;
                await this.AlbumMusics.AddRangeAsync(_innerAlbum.Musics);
                RaiseAllPropertyChanged();
            }
            else
            {
                //todo 网络相关提示信息
            }

        }
        private async void SelectedCommandExecute(IEnumerable items)
        {
            if (IsSelectedModel)
            {
                this._selectedMusics = items.Cast<Music>().ToArray(); return;
            }
            else if (_selectedMusics == null)
            {
                var temp = items.Cast<Music>().FirstOrDefault();
                if (temp != null)
                    await this._playPart.Play(temp);
                return;
            }

            await this._playPart.MusicsListCollection.AddRangeAsync(_selectedMusics, async source => await this._playPart.Play(source[0]));
            _selectedMusics = null;
        }
        private async void PlayAllCommandExecute()
        {
            if (this.AlbumMusics.Count == 0)
            {
                this._playPart.MusicsListCollection.Clear();
                this._playPart.Stop();
                return;
            }
            await this._playPart.MusicsListCollection.AddRangeAsync(AlbumMusics, async range => await this._playPart.Play(range.First()));

        }
        /// <summary>
        /// 表示封面图片
        /// </summary>
        public string ConverPic => _innerAlbum?.PicUrl;
        public string AlbumName => _innerAlbum?.Name;
        /// <summary>
        /// 发行时间
        /// </summary>
        public DateTime CreateDate => (_innerAlbum?.CreateDate).GetValueOrDefault();
        public ObservableCollection<Artist> Artists { get; } = new ObservableCollection<Artist>();
        /// <summary>
        /// 点击歌手名称的命令
        /// </summary>
        public ICommand ArtistNameCommand { get; }
        public int CollectionCount => _innerAlbum?.CollectionCount ?? 0;

        public int CommentCount => _innerAlbum?.CommentCount ?? 0;
        public string Description => _innerAlbum?.Description;
        public ICommand PlayAllCommand { get; }
        public bool IsSelectedModel
        {
            get { return _isSelectedModel; }
            set { SetProperty(ref _isSelectedModel, value); }
        }
        public ICommand SelectedCommand { get; }
        public ObservableCollection<Music> AlbumMusics { get; } = new ObservableCollection<Music>();
    }
}
