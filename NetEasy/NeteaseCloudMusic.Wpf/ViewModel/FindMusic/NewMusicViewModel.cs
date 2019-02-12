using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using NeteaseCloudMusic.Wpf.Model;
using System.Windows.Input;
using CommonServiceLocator;
using Prism.Commands;
using Prism.Regions;
using System.Windows.Controls;
using NeteaseCloudMusic.Global.Enums;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 最新音乐tab页面对应的viewmodel
    /// </summary>
    public class NewMusicViewModel : BindableBase
    {
       

       // private readonly MainWindowViewModel _mainWindowViewModel;

        private ObservableCollection<MusicModel> _newMusicList = new ObservableCollection<MusicModel>();
        private bool _isSelectModel;
        private MusicModel[] selectedMusics;
        private void NewMusicOrDiskCommandImpl( string msg)
        {
            Console.WriteLine(msg);
        }
        private void PlayAllOrSelectedCommandImpl()
        {
            if (!IsSelectModel)
            {

               
            }


        }

        private void  LanguageCommandImpl( string lagu)
        {
            Console.WriteLine(lagu);
        }
        public NewMusicViewModel(MainWindowViewModel mainViewModel)
        {
            for (int i = 0; i < 10; i++)
            {
                _newMusicList.Add(new MusicModel
                {
                    Title = ((char)(i + 34585)) + i.ToString("000000"),
                    IsLike = i % 2 == 0,
                   // Index = i,
                    ArtistName = "bug" + i,
                    AlbumName = "sdahgfjfjkepwtjmrlgujbiphmrkltjwealgmndsalyjre;jhbmfdshr",
                    //MusicQuality = (MusicQualityLevel)(i % 3),
                    Duration = TimeSpan.FromMinutes(i * 8.1+2)
                });
            }
            PlayAllOrSelectedCommand =new DelegateCommand( PlayAllOrSelectedCommandImpl);
            NewMusicOrDiskCommand = new DelegateCommand<string >(NewMusicOrDiskCommandImpl);
            LanguageCommand = new DelegateCommand<string>( LanguageCommandImpl);
            SelectedCommand = new DelegateCommand<IEnumerable>(SelectedCommandImpl );
        }
        /// <summary>
        /// 选择完成后需要执行的方法
        /// </summary>
        /// <param name="items"></param>
        private void SelectedCommandImpl( IEnumerable items)
        {
            if (IsSelectModel)
            {
                selectedMusics =items.Cast<MusicModel>().ToArray();
                return;
            }
            if (selectedMusics == null)
                return;
            //_mainWindowViewModel.CurrentPlayList.Clear();
            //_mainWindowViewModel.CurrentPlayList.AddRange(selectedMusics);
            //_mainWindowViewModel.PlayFirstOrDefault();
            selectedMusics = null;

        }
        /// <summary>
        /// 最新音乐列表
        /// </summary>
        public ObservableCollection<MusicModel> NewMusicList
        {
            get
            {
                return _newMusicList;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SetProperty(ref _newMusicList, value);
            }

        }
        /// <summary>
        /// 获取或设置当前页面是用户选择还是播放全部
        /// </summary>
        public bool IsSelectModel
        {
            get
            {
                return _isSelectModel;
            }

            set
            {
                if (value == _isSelectModel) return;
                SetProperty(ref _isSelectModel, value);

                RaisePropertyChanged(nameof(SelectionMode));


            }
        }
        /// <summary>
        /// 用来绑定到页面是否是选择状态以避免额外的值转换器
        /// </summary>
        public SelectionMode SelectionMode => _isSelectModel ? SelectionMode.Extended : SelectionMode.Single;
        /// <summary>
        /// 播放全部或者选中的音乐
        /// </summary>
        public ICommand PlayAllOrSelectedCommand { get; }
        /// <summary>
        /// 在选择的时候执行的命令
        /// </summary>
        public ICommand SelectedCommand { get; }
        /// <summary>
        /// 新歌或者新碟对应的命令
        /// </summary>
        public ICommand NewMusicOrDiskCommand { get; }

        /// <summary>
        /// 语言对应的命令
        /// </summary>
        public ICommand LanguageCommand { get; }
    }
}
