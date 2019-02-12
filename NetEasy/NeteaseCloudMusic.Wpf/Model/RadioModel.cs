using System.Windows.Media;
using Prism.Mvvm;
using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Wpf.Model
{
    /// <summary>
    /// 代表电台的绑定model
    /// </summary>
    public class RadioModel: NotifyPropertyChangedModelBase<Radio>
    {
        public RadioModel():base()
        {

        }
        public RadioModel(Radio radio):base(radio)
        {

        }
        /// <summary>
        /// radio图片
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
        /// 电台的标题
        /// </summary>
        public string Title
        {
            get
            {
                return innerModel.Title;
            }

            set { innerModel.Title =value ;
                RaisePropertyChanged();
            }
        }
       
        /// <summary>
        /// 表示主播名
        /// </summary>
        public string AnchorName
        {
            get { return innerModel.CopyWriter; }

            set { innerModel.CopyWriter = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 电台的描述
        /// </summary>
        public string Description
        {
            get
            {
                return innerModel.Description;
            }

            set { innerModel.Description = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 电台的订阅数量
        /// </summary>
        public int SubCount
        {
            get
            {
                return innerModel.SubscribedCount;
            }

            set { innerModel.SubscribedCount = value;
                RaisePropertyChanged();
            }
        }

    }
}