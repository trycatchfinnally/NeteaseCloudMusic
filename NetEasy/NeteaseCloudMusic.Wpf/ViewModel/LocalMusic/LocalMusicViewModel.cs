using NeteaseCloudMusic.Wpf.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class LocalMusicViewModel : BindableBase
    {
        private bool _localMusicIsInSelectionMode;
        public LocalMusicViewModel()
        {
            AllMusic.CollectionChanged += AllMusic_CollectionChanged;

            for (int i = 0; i < 100; i++)
            {
                // if ('0' + i != 'Z' && '0' + i != 'z')
                AllMusic.Add(new Model.LocalMusicModel
                {
                    AlbumName = (char)('中' + i) + "album" + i,
                    ArtistName = (char)('狗' + i) +"Artis" + i,
                    Title = (char)('牛' + i) + "title",
                    IsLike = i % 3 == 0,
                    Duration = TimeSpan.FromMilliseconds(i * 5452)
                });
            }
            
            AllMusic.Insert(0, new Model.LocalMusicModel { AlbumName = "草album", ArtistName = "草artist", Title = "草title" });
            AllMusic.Last().FilePath = @"D:\VSSFiles\主档Source Review 模板.xlsx";
            //MusicCollection.AddRange(AllMusic.OrderBy(x => x.Title[0]).Skip(20));
            //ArtisCollection.AddRange(AllMusic.OrderBy(x => x.ArtistName[0]));
            //AlbumCollection.AddRange(AllMusic.OrderBy(x => x.AlbumName[0]));
            PlayAllLocalMusicCommand = new DelegateCommand(PlayAllLocalMusicCommandImpl);
            ChooseDirectoryCommand = new DelegateCommand(ChooseDirectoryCommandImpl);
        }

        private void AllMusic_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    MusicCollection.AddRange(e.NewItems.Cast<LocalMusicModel>());
                    ArtisCollection.AddRange(e.NewItems.Cast<LocalMusicModel>());
                    AlbumCollection.AddRange(e.NewItems.Cast<LocalMusicModel>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (LocalMusicModel item in e.OldItems)
                    {
                        MusicCollection.Remove(item);
                        ArtisCollection.Remove(item);
                        AlbumCollection.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    MusicCollection.Clear();
                    ArtisCollection.Clear();
                    AlbumCollection.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           // throw new NotImplementedException();
        }

        private void PlayAllLocalMusicCommandImpl()
        {
             
        }
        private async void ChooseDirectoryCommandImpl()
        {
            var fd = new System.Windows.Forms.FolderBrowserDialog();
            string selectPath ;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.AllMusic.Clear();
                selectPath = fd.SelectedPath;
            }
            else return;
            var result=  await  Task.Factory.StartNew(( path) =>
            {
                var temp = path.ToString();

                if (!string.IsNullOrEmpty(temp))
                {
                    List<string> files = new List<string>();
                  var extensions=  Context.SupportFileExtension.Split('|');
                    foreach (var item in extensions)
                    {
                        files.AddRange(Directory.GetFiles(temp, item, SearchOption.AllDirectories));
                    }
                    return files.ToArray();
                }
                return Array.Empty<string>();
            }, state:selectPath);
            foreach (var item in result)
            {
                AllMusic.Add(new Model.LocalMusicModel { FilePath=item,
                    AlbumName =Path.GetFileName(item),
                    Title= Path.GetFileName(item),
                    ArtistName= Path.GetFileName(item)
                });
            }
            
        }
        private ObservableCollection<LocalMusicModel> AllMusic { get; } = new ObservableCollection<LocalMusicModel>();

        /// <summary>
        /// 歌曲
        /// </summary>
        public ObservableCollection<LocalMusicModel> MusicCollection { get; } = new ObservableCollection<LocalMusicModel>();
        /// <summary>
        /// 按歌手分类
        /// </summary>
        public ObservableCollection<LocalMusicModel> ArtisCollection { get; } = new ObservableCollection<LocalMusicModel>();
        /// <summary>
        /// 按专辑分类
        /// </summary>
        public ObservableCollection<LocalMusicModel> AlbumCollection { get; } = new ObservableCollection<LocalMusicModel>();
        /// <summary>
        /// 播放所有本地音乐
        /// </summary>
        public ICommand PlayAllLocalMusicCommand { get; }
     /// <summary>
     /// 用于绑定到集合的选择属性，避免使用值转换器
     /// </summary>
        public SelectionMode LocalMusicSelectionMode => LocalMusicIsInSelectionMode ? SelectionMode.Extended : SelectionMode.Single;
        /// <summary>
        /// 本地音乐是否处于选择状态
        /// </summary>
        public bool LocalMusicIsInSelectionMode
        {
            get
            {
                return _localMusicIsInSelectionMode;
            }

            set
            {
                if (value == _localMusicIsInSelectionMode) return;
                SetProperty(ref _localMusicIsInSelectionMode, value);
                RaisePropertyChanged(nameof(LocalMusicSelectionMode));
            }
        }
        /// <summary>
        /// 选择文件夹执行的命令
        /// </summary>
        public ICommand ChooseDirectoryCommand { get; }
    }
}
