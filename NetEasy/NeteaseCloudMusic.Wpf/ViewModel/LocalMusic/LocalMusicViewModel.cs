using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Wpf.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NeteaseCloudMusic.Wpf.Properties;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class LocalMusicViewModel : BindableBase
    {
        private readonly NeteaseCloudMusic.Services.LocalFile.IFileServices _fileServices;
        private bool _localMusicIsInSelectionMode;
        public LocalMusicViewModel(NeteaseCloudMusic.Services.LocalFile.IFileServices fileServices)
        {
            /*
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
            MusicCollection.AddRange(AllMusic.OrderBy(x => x.Title[0]).Skip(20));
            ArtisCollection.AddRange(AllMusic.OrderBy(x => x.ArtistName[0]));
            AlbumCollection.AddRange(AllMusic.OrderBy(x => x.AlbumName[0]));
           */
            this._fileServices = fileServices;
            AllMusic.CollectionChanged += AllMusic_CollectionChanged;
            PlayAllLocalMusicCommand = new DelegateCommand(PlayAllLocalMusicCommandImpl);
            ChooseDirectoryCommand = new DelegateCommand(ChooseDirectoryCommandImpl);
        }

        private async void AllMusic_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    MusicCollection.AddRange(e.NewItems.Cast<LocalMusic>());
                    // ArtistCollection.AddRange(e.NewItems.Cast<LocalMusic>());
                    AlbumCollection.AddRange(e.NewItems.Cast<LocalMusic>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (LocalMusic item in e.OldItems)
                    {
                        MusicCollection.Remove(item);
                        //  ArtistCollection.Remove(item);
                        AlbumCollection.Remove(item);
                    }
                    await ArtistCollection.AddRangeAsync(AllMusic.GroupBy(x => x.ArtistsName?.FirstOrDefault() ?? "未知艺术家").ToDictionary(x => x.Key, x => x.ToArray())
                        .Select(x =>
                            new LocalArtist
                            {
                                Name = x.Key,
                                PicPath = x.Value.First().Id3Pic,
                                LocalMusics = x.Value
                            }));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    MusicCollection.Clear();
                    ArtistCollection.Clear();
                    AlbumCollection.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }

        private void PlayAllLocalMusicCommandImpl()
        {

        }
        private async void ChooseDirectoryCommandImpl()
        {
            string selectPath = this._fileServices.SelectDirectory();
            if (string.IsNullOrEmpty(selectPath))
            {
                return;
            }

            AllMusic.Clear();
            var result = await Task.Factory.StartNew((path) =>
            {
                var temp = path.ToString();

                if (!string.IsNullOrEmpty(temp))
                {
                    List<string> files = new List<string>();
                    var extensions =Settings.Default.SupportFileExtension.Split('|');
                    foreach (var item in extensions)
                    {
                        files.AddRange(this._fileServices.GetFiles(temp, true, item));
                    }
                    return files.ToArray();
                }
                return Array.Empty<string>();
            }, state: selectPath);
            int totalPage = (int)Math.Ceiling(result.Length / (double)Settings.Default.LimitPerPage);
            var tasks = new List<Task<LocalMusic[]>>();
            for (int i = 0; i < totalPage; i++)
            {
                var tmp = result.Skip(i * Settings.Default.LimitPerPage).Take(Settings.Default.LimitPerPage);
                tasks.Add(SelectMethod(tmp));
            }

            await Task.WhenAll(tasks);
            await AllMusic.AddRangeAsync(tasks.AsParallel().SelectMany(x => x.Result).OrderBy(x => x.Title),
               async y => await ArtistCollection.AddRangeAsync(AllMusic.GroupBy(x => x.ArtistsName?.FirstOrDefault() ?? "未知艺术家").ToDictionary(x => x.Key, x => x.ToArray())
                .Select(x =>
                    new LocalArtist
                    {
                        Name = x.Key,
                        PicPath = x.Value.First().Id3Pic,
                        LocalMusics = x.Value
                    })));
            //foreach (var item in result)
            //{

            //    var (title, albumName, artistName, duration, fileSize) = this._fileServices.GetFileId3(item);

            //    AllMusic.Add(new LocalMusic
            //    {
            //        FilePath = item,
            //        AlbumName = albumName,
            //        Title = title,
            //        ArtistsName = artistName,
            //        Duration = duration,
            //        FileSize = fileSize
            //    });
            //}

        }

        private Task<LocalMusic[]> SelectMethod(IEnumerable<string> enumer)
        {
            return Task.Factory.StartNew(() =>
            {
                return enumer.Select(item =>
                {
                    var (title, albumName, artistName, duration, fileSize, picPath) =
                        this._fileServices.GetFileId3(item);
                    return new LocalMusic
                    {
                        FilePath = item,
                        AlbumName = albumName,
                        Title = title,
                        ArtistsName = artistName.ToArray(),
                        Duration = duration,
                        FileSize = fileSize,
                        Id3Pic = picPath
                    };
                }).ToArray();
            });
        }
        private ObservableCollection<LocalMusic> AllMusic { get; } = new ObservableCollection<LocalMusic>();

        /// <summary>
        /// 歌曲
        /// </summary>
        public ObservableCollection<LocalMusic> MusicCollection { get; } = new ObservableCollection<LocalMusic>();
        /// <summary>
        /// 按歌手分类
        /// </summary>
        public ObservableCollection<LocalArtist> ArtistCollection { get; } = new ObservableCollection<LocalArtist>();
        /// <summary>
        /// 按专辑分类
        /// </summary>
        public ObservableCollection<LocalMusic> AlbumCollection { get; } = new ObservableCollection<LocalMusic>();
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
                return this._localMusicIsInSelectionMode;
            }

            set
            {
                if (value == this._localMusicIsInSelectionMode) return;
                SetProperty(ref this._localMusicIsInSelectionMode, value);
                RaisePropertyChanged(nameof(LocalMusicSelectionMode));
            }
        }
        /// <summary>
        /// 选择文件夹执行的命令
        /// </summary>
        public ICommand ChooseDirectoryCommand { get; }
    }
}
