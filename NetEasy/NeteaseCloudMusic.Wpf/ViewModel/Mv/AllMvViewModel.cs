using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Enums;
 
using NeteaseCloudMusic.Wpf.Model;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
   public  class AllMvViewModel:BindableBase
   {
       private LanguageType _selectedLanguageType;
       private MvType _mvType;
       private MvOrderbyType _mvOrderbyType;

       public AllMvViewModel()
       {
           for (int i = 0; i < 30; i++)
           {
               Mvs.Add(new Model.MvModel { Title="sadhgasdetwtgdsa",ArtistName="setagds",WatchCount=454768*i});
           }
       }
        /// <summary>
        /// 当检索条件变化的时候执行的方法
        /// </summary>
       private void OnSearchFilterChanged()
       {

       }
        /// <summary>
        /// 选择的区域
        /// </summary>
        public LanguageType SelectedLanguageType
        {
            get
            {
                return _selectedLanguageType;
            }

            set
            {
                SetProperty(ref _selectedLanguageType, value);
                OnSearchFilterChanged();
            }
        }
        /// <summary>
        /// 代表mv的类型
        /// </summary>
        public MvType MvType
        {
            get
            {
                return _mvType;
            }

            set { SetProperty(ref _mvType, value); OnSearchFilterChanged(); }
        }
        /// <summary>
        /// 代表排序类型
        /// </summary>
        public MvOrderbyType MvOrderbyType
        {
            get
            {
                return _mvOrderbyType;
            }

            set { SetProperty(ref _mvOrderbyType, value); OnSearchFilterChanged(); }
        }
        /// <summary>
        /// 检索出来的mv
        /// </summary>
       public ObservableCollection<MvModel> Mvs { get; } = new ObservableCollection<MvModel>();
   }
}
