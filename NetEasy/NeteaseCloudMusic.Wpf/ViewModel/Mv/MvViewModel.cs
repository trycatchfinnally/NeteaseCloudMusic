using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// mv页面对应的viewmodel
    /// </summary>
   public  class MvViewModel:BindableBase
    {
        private int _selectedIndex ;
        private FeaturedViewModel _featuredViewModel;
        private MvLeaderboardViewModel _mvLeaderboardViewModel;
        private AllMvViewModel _allMvViewModel;
        public MvViewModel()
        {
            SelectedIndex = 0;
        }
        /// <summary>
        /// 选中的项目序号
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                switch (value )
                {
                    case 0:
                        if (FeaturedViewModel == null)
                            FeaturedViewModel =  ServiceLocator.Current.TryResolve<FeaturedViewModel >();
                        break;
                    case 1:
                        if (MvLeaderboardViewModel == null)
                            MvLeaderboardViewModel = ServiceLocator.Current.TryResolve<MvLeaderboardViewModel>();
                        break;
                    case 2:
                        if (AllMvViewModel == null)
                            AllMvViewModel = ServiceLocator.Current.TryResolve<AllMvViewModel>();
                        break;
                    default: throw new IndexOutOfRangeException("一共就只有3个页面。");
                }
                SetProperty(ref _selectedIndex, value);

            }
        }

        public FeaturedViewModel FeaturedViewModel
        {
            get
            {
                return _featuredViewModel;
            }

            private set { SetProperty(ref _featuredViewModel, value); }
        }

        public MvLeaderboardViewModel MvLeaderboardViewModel
        {
            get
            {
                return _mvLeaderboardViewModel;
            }

            private set { SetProperty(ref _mvLeaderboardViewModel, value); }
        }

        public AllMvViewModel AllMvViewModel
        {
            get
            {
                return _allMvViewModel;
            }

          private   set { SetProperty(ref _allMvViewModel, value); }
        }
    }
}
