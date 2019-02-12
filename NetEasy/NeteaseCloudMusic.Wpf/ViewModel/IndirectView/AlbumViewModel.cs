using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class AlbumViewModel : IndirectViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly IRegionManager _navigationService;
        private Global.Model.Album _innerAlbum = new Global.Model.Album();
        private bool _isSelectedModel;
        private Music[] selectedMusics;
        public AlbumViewModel(INetWorkServices netWorkServices,
            IRegionManager navigationService)
        {
            this._netWorkServices = netWorkServices;
            this._navigationService = navigationService;
            PlayAllCommand = new DelegateCommand(PlayAllCommandExecute);
            this.SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);

        }
        protected override async void SetById(long id)
        {
            var json = await this._netWorkServices.GetAsync("Album", "GetAlbumDetailById", new { id });
            _innerAlbum = JsonConvert.DeserializeObject<Album>(json);
            await this.AlbumMusics.AddRangeAsync(_innerAlbum.Musics);
            RaiseAllPropertyChanged();
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
        private async void PlayAllCommandExecute()
        {
            if (this.AlbumMusics.Count == 0)
            {
                Context.CurrentPlayMusics.Clear();
                return;
            }
            await Context.CurrentPlayMusics.AddRangeAsync(AlbumMusics, range => Context.PlayCommand.Execute(range.First()));

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
