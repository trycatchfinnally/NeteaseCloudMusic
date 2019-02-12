using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Mvvm;
using NeteaseCloudMusic.Wpf.Model;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class DownloadedMusicViewModel : BindableBase
    {

        private bool _downloadedMusicIsInSelectionMode;
        public DownloadedMusicViewModel()
        {
            for (int i = 0; i < 100; i++)
            {
                // if ('0' + i != 'Z' && '0' + i != 'z')
                //DownloadedMusicCollection.Add(new Model.LocalMusicModel
                //{
                //    AlbumName = (char)('中' + i) + "album" + i,
                //    ArtistName = (char)('狗' + i) + "Artis" + i,
                //    Title = (char)('牛' + i) + "title",
                //    IsLike = i % 3 == 0,
                //    Duration = TimeSpan.FromMilliseconds(i * 5452)
                //});
            }
        }
        public ObservableCollection<LocalMusicModel> DownloadedMusicCollection { get; } =
            new ObservableCollection<LocalMusicModel>();

        public bool DownloadedMusicIsInSelectionMode
        {
            get
            {
                return _downloadedMusicIsInSelectionMode;
            }

            set
            {
                SetProperty(ref _downloadedMusicIsInSelectionMode, value);
                RaisePropertyChanged(nameof(DownloadedMusicSelectionMode));
            }
        }
        /// <summary>
        /// ;播放全部的命令
        /// </summary>
        public ICommand PlayAllDownloadedMusicCommand { get; }

        public SelectionMode DownloadedMusicSelectionMode =>
            DownloadedMusicIsInSelectionMode ? SelectionMode.Extended : SelectionMode.Single;
    }
}
