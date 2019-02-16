using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows.Media;
using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Wpf.Model
{
    /// <summary>
    /// 代表MV的绑定Model
    /// </summary>
   public  class MvModel: NotifyPropertyChangedModelBase<Mv>
    {
        public MvModel():base()
        {

        }
        public MvModel(Mv innermodel):base(innermodel)
        {

        }
        /// <summary>
        /// mv图片
        /// </summary>
        public string  Picture
        {
            get
            {
                return innerModel.PicUrl;
            }

            set { innerModel.PicUrl = value; }
        }

        /// <summary>
        /// mv的标题
        /// </summary>
        public string Title
        {
            get { return innerModel.Name; }

            set { innerModel.Name = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 表示歌手名
        /// </summary>
        public string ArtistName
        {
            get
            {
                return innerModel.ArtistName;
            }

            set {  
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// mv被播放次数
        /// </summary>
        public int WatchCount
        {
            get
            {
                return (int)innerModel.PlayCount;
            }

            set { innerModel.PlayCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// mv的描述
        /// </summary>
        public string Description
        {
            get
            {
                return innerModel.Desc;
            }

            set { innerModel.Desc=value ;
                RaisePropertyChanged();
            }
        }
        
    }
}
