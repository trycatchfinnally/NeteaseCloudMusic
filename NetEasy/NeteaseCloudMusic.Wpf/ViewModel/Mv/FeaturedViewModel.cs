using NeteaseCloudMusic.Wpf.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Windows.Input;
using NeteaseCloudMusic.Global.Enums;
using Prism.Commands;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 精选页面对应的viewmodel
    /// </summary>
   public  class FeaturedViewModel:BindableBase
    {
        private LanguageType _languageType;
        public FeaturedViewModel()
        {
            for (int i = 0; i < 3; i++)
            {
                NeteaseProduceds.Add(new Model.MvModel {Title = i + "asdhgreygf", ArtistName = "cegsdh" + i});
                NewMvs.Add(new Model.MvModel {Title = i + "asdhgreygf", ArtistName = "cegsdh" + i,Description="4ergadshreyaSgdst"});
            }
            MoreCommand = new DelegateCommand<string >(MoreCommandImpl);
        }
        /// <summary>
        /// 点击更多时候执行的命令
        /// </summary>
        /// <param name="type"></param>
        private void MoreCommandImpl(string  type)
        {
            switch (type)
            {
                case "1":
                    break;
                case "2":
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
        /// <summary>
        /// 语种发生变化的时候请求最新MV的数据
        /// </summary>
        private void RequstNewMvs()
        {
            Console.WriteLine(LanguageType);
        }
        public LanguageType LanguageType
        {
            get
            {
                return _languageType;
            }

            set
            {
                SetProperty(ref _languageType, value);
                RequstNewMvs();
            }
        }
       
        /// <summary>
        /// 网易出品
        /// </summary>
        public ObservableCollection<MvModel> NeteaseProduceds { get; } = new ObservableCollection<MvModel>();
        /// <summary>
        /// 最新mv
        /// </summary>
        public ObservableCollection<MvModel> NewMvs { get; } = new ObservableCollection<MvModel>();
        /// <summary>
        /// 更多对应的命令
        /// </summary>
        public ICommand MoreCommand { get; }
    }
}
