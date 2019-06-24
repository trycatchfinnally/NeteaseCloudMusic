using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Extensions;
using NeteaseCloudMusic.Wpf.Model;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NeteaseCloudMusic.Wpf.Proxy;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    public class PlayListDetailViewModel : IndirectViewModelBase
    {
        private PlayListDetail _innerPlayList;
        private readonly INetWorkServices _netWorkServices;
        private readonly PlayPartCore _playPart;
        private bool _isSelectedModel;
        private ObservableCollection<Music> _tracks;
        private ObservableCollection<Music> _displayTracks;
        private string _serachKeyword;
        private Music[] _selectedMusics;
        // public  event EventHandler RefreshCompleated  ;

        public PlayListDetailViewModel(INetWorkServices netWorkServices, PlayPartCore playPart)
        {
            this._netWorkServices = netWorkServices;
            _playPart = playPart;
            _innerPlayList = new PlayListDetail();
            PlayAllCommand = new DelegateCommand(PlayAllCommandExecute);
            this.SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandExecute);
        }



        /// <summary>
        /// 设置当前的播放列表id
        /// </summary>
        /// <param name="id"></param>
        protected override async void SetById(long id)
        {
            Console.WriteLine(id);
            var netWorkDataResult = await this._netWorkServices.GetAsync<PlayListDetail>("Common", "GetPlaylistById", new { id });
            if (netWorkDataResult.Successed)
            {
                var model = netWorkDataResult.Data;
                this._innerPlayList = model;
                _tracks = null;//更新数据
                OnSearchKeyWordChanged();
                RaiseAllPropertyChanged();
            }
            else
            {
                //todo 网络连接失败
            }
        }
        private async void PlayAllCommandExecute()
        {
         _playPart.MusicsListCollection.Clear();
         _playPart.Stop();
            if (DisplayTracks.Count == 0) return;
            await _playPart.MusicsListCollection.AddRangeAsync(DisplayTracks, async range =>await  _playPart.Play (range.First()));

        }
        private async void OnSearchKeyWordChanged()
        {

            IEnumerable<Music> musicModels;
            if (string.IsNullOrEmpty(SearchKeyWord))
            {
                musicModels = Tracks;
            }
            else
            {

                musicModels = Tracks.AsParallel().Where(x => (x.Name?.IndexOf(SearchKeyWord, StringComparison.OrdinalIgnoreCase)) > -1
                  || (x.ArtistName?.IndexOf(SearchKeyWord, StringComparison.OrdinalIgnoreCase)) > -1
                  || (x.AlbumName?.IndexOf(SearchKeyWord, StringComparison.OrdinalIgnoreCase)) > -1
                   ).ToArray();
            }
            await DisplayTracks.AddRangeAsync(musicModels);
            //const int CountPerPage = 30;
            //int pageCount =(int)Math.Ceiling( musicModels.Count() /(double) CountPerPage);
            //DisplayTracks.Clear();
            //for (int i = 0; i < pageCount; i++)
            //{
            //    DisplayTracks.AddRange(musicModels.Skip(i * CountPerPage).Take(30));
            //    await Task.Delay(150);
            //}

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
                    await _playPart.Play(temp);
                return;
            }

            await _playPart.MusicsListCollection.AddRangeAsync(_selectedMusics, async source => await _playPart.Play (source.First()));
            _selectedMusics = null;
        }








        /// <summary> 

        /// 对应图片
        /// </summary>
        public string PlayListPic
        {
            get { return _innerPlayList.PicUrl; }

        }
        /// <summary>
        /// 对应名称
        /// </summary>
        public string PlayListName => _innerPlayList.Name;
        public User CreateUser => _innerPlayList.CreateUser ?? (_innerPlayList.CreateUser = new User());
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate => _innerPlayList.CreateDate;
        public int CollectionCount => _innerPlayList.CollectionCount;
        public int CommentCount => _innerPlayList.CommentCount;
        public List<string> Tags => _innerPlayList.Tags;
        public string Description => _innerPlayList.Description;
        /// <summary>
        /// 播放全部对应的命令
        /// </summary>
        public ICommand PlayAllCommand { get; }
        /// <summary>
        /// 播放选中项的命令
        /// </summary>
        public ICommand SelectedCommand { get; }
        public bool IsSelectedModel
        {
            get { return _isSelectedModel; }
            set
            {
                if (value == _isSelectedModel) return;
                SetProperty(ref _isSelectedModel, value);
                RaisePropertyChanged(nameof(SelectionMode));
            }
        }
        public SelectionMode SelectionMode => _isSelectedModel ? SelectionMode.Multiple : SelectionMode.Single;
        public ObservableCollection<Music> Tracks
        {
            get
            {
                if (this._tracks == null)
                {
                    _tracks = new ObservableCollection<Music>();
                    if (_innerPlayList.Musics != null && _innerPlayList.Musics.Count > 0)
                        _tracks.AddRange(_innerPlayList.Musics);
                }
                return this._tracks;
            }
        }

        public ObservableCollection<Music> DisplayTracks
        {
            get
            {
                if (_displayTracks == null)
                    _displayTracks = new ObservableCollection<Music>();
                return _displayTracks;
            }

        }
        /// <summary>
        /// 搜索的关键字 
        /// </summary>
        public string SearchKeyWord
        {
            get
            {

                return this._serachKeyword;
            }
            set
            {
                if (value == _serachKeyword) return;
                SetProperty(ref _serachKeyword, value);
                OnSearchKeyWordChanged();
            }
        }
    }
}
