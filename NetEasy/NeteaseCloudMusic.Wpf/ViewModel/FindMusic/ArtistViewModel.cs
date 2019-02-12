using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeteaseCloudMusic.Global.Enums;
using NeteaseCloudMusic.Wpf.Model;
using Prism.Commands;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 绑定到发现音乐的歌手tab页面的viewmodel
    /// </summary>
    public class ArtistViewModel : BindableBase
    {
        private LanguageType _selectedLanguageType = LanguageType.All;
        private ArtistType _selectedArtistType = ArtistType.All;
        private ChinesePinYinHeadType _pinYinHeadType = ChinesePinYinHeadType.Wildcard;
        private ObservableCollection<ArtistModel> _artists = new ObservableCollection<ArtistModel>();
        public ArtistViewModel()
        {
            OnSelectedFilterChanged();
        }
        /// <summary>
        /// 档选中的语种、歌手类型、字母发生变化的时候触发
        /// </summary>
        private void OnSelectedFilterChanged()
        {
            Artists.Clear();
            for (int i = 0; i < 30; i++)
            {
                Artists.Add(new ArtistModel { Name=SelectedLanguageType+","+ SelectedArtistType +","+ PinYinHeadType });
            }
        }
        /// <summary>
        /// 选中的语种
        /// </summary>
        public LanguageType SelectedLanguageType
        {
            get
            {
                return _selectedLanguageType;
            }

            set
            {
                if (_selectedLanguageType == value) return;
                SetProperty(ref _selectedLanguageType, value);
                OnSelectedFilterChanged();
            }
        }
        /// <summary>
        /// 代表选中的歌手类型
        /// </summary>
        public ArtistType SelectedArtistType
        {
            get
            {
                return _selectedArtistType;
            }

            set
            {

                if (value == _selectedArtistType) return;
                SetProperty(ref _selectedArtistType, value);
                OnSelectedFilterChanged();
            }
        }
        /// <summary>
        /// 代表按字母进行筛选
        /// </summary>
        public ChinesePinYinHeadType PinYinHeadType
        {
            get
            {
                return _pinYinHeadType;
            }

            set
            {
                if (value == _pinYinHeadType) return;
                SetProperty(ref _pinYinHeadType, value);
                OnSelectedFilterChanged();
            }
        }
        /// <summary>
        /// 根据筛选结果查询得到的集合
        /// </summary>
        public ObservableCollection<ArtistModel> Artists
        {
            get
            {
                return _artists;
            }

            set { SetProperty(ref _artists, value); }
        }
    }
}
