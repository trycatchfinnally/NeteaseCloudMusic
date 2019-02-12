using NeteaseCloudMusic.Services.NetWork;
using Prism.Mvvm;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    public class FindMusicViewModel : NavigationViewModelBase
    {
        private readonly INetWorkServices _netWorkServices;
        private bool _isPersonalityRecommendViewModelActived;
        private bool _isMusicListViewModelActived;
        private bool _isAnchorRadioViewModelActived;
        private bool _isNewMusicViewModelActived;
        private bool _isArtistViewModelActived;
        private PersonalityRecommendViewModel _personalityRecommendViewModel;
        private MusicListViewModel _musicListViewModel;
        private AnchorRadioViewModel _anchorRadioViewModel;
        private NewMusicViewModel _newMusicViewModel;
        private ArtistViewModel _artistViewModel;
        public FindMusicViewModel(INetWorkServices netWorkServices)
        {
            _netWorkServices = netWorkServices;
            IsPersonalityRecommendViewModelActived = true;
        }
        public bool IsPersonalityRecommendViewModelActived
        {
            get { return _isPersonalityRecommendViewModelActived; }
            set
            {
                SetProperty(ref _isPersonalityRecommendViewModelActived, value);
                if (_personalityRecommendViewModel == null && value)
                {
                    _personalityRecommendViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<PersonalityRecommendViewModel>();
                    RaisePropertyChanged(nameof(RecommendViewModel));
                }
            }
        }

        /// <summary>
        /// 个性推荐页面的viewmodel
        /// </summary>
        public PersonalityRecommendViewModel RecommendViewModel
        {
            get { return _personalityRecommendViewModel; }
            set { SetProperty(ref _personalityRecommendViewModel, value); }
        }

        public bool IsMusicListViewModelActived
        {
            get { return _isMusicListViewModelActived; }
            set
            {
                SetProperty(ref _isMusicListViewModelActived, value);
                if (_musicListViewModel == null && value)
                {
                    _musicListViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<MusicListViewModel>();
                    this.RaisePropertyChanged(nameof(MusicListViewModel));
                }

            }
        }

        ///// <summary>
        ///// 歌单页面的viewmodel
        ///// </summary>
        public MusicListViewModel MusicListViewModel
        {
            get { return _musicListViewModel; }
            set { SetProperty(ref _musicListViewModel, value); }
        }


        public bool IsAnchorRadioViewModelActived
        {
            get { return _isAnchorRadioViewModelActived; }
            set
            {
                SetProperty(ref _isAnchorRadioViewModelActived, value);
                if (value && _anchorRadioViewModel == null)
                {
                    _anchorRadioViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<AnchorRadioViewModel>();
                    RaisePropertyChanged(nameof(AnchorRadioViewModel));
                }
            }
        }

        /// <summary>
        /// 主播电台页面的viewmodel
        /// </summary>
        public AnchorRadioViewModel AnchorRadioViewModel
        {
            get { return _anchorRadioViewModel; }
            set { SetProperty(ref _anchorRadioViewModel, value); }
        }


        public bool IsNewMusicViewModelActived
        {
            get
            {
                return _isNewMusicViewModelActived;
            }

            set
            {
                SetProperty(ref _isNewMusicViewModelActived, value);
                if (value && _newMusicViewModel == null)
                    NewMusicViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<NewMusicViewModel>();
            }
        }
        /// <summary>
        /// 最新音乐页面viewmodel
        /// </summary>
        public NewMusicViewModel NewMusicViewModel
        {
            get
            {
                return _newMusicViewModel;
            }

            set { SetProperty(ref _newMusicViewModel, value); }
        }

        public bool IsArtistViewModelActived
        {
            get
            {
                return _isArtistViewModelActived;
            }

            set
            {
                SetProperty(ref _isArtistViewModelActived, value);
                if (value && _artistViewModel == null)
                    ArtistViewModel = new ArtistViewModel();
            }

        }
        /// <summary>
        /// 歌手页面对应的viewmodel
        /// </summary>
        public ArtistViewModel ArtistViewModel
        {
            get
            {
                return _artistViewModel;
            }

            set { SetProperty(ref _artistViewModel, value); }
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Id"))
            {

            }
        }
    }
}
