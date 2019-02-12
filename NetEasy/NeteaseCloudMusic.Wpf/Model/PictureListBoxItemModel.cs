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
    public class PictureListBoxItemModel : BindableBase
    {
        private readonly PictureListBoxItem _innerModel  ;
        public PictureListBoxItemModel():this(new PictureListBoxItem ())
        {

        }
        public PictureListBoxItemModel(PictureListBoxItem innerModel)
        {
            this._innerModel = innerModel;
        }
        /// <summary>
        /// 获取或设置显示的图片
        /// </summary>
        public string  ImageSource
        {
            get { return _innerModel.ImageSource; }
            set { _innerModel.ImageSource = value;RaisePropertyChanged(); }

        }
       
        /// <summary>
        /// 获取或设置显示的项描述
        /// </summary>
        public string Text
        {
            get { return _innerModel.Text; }
            set { _innerModel.Text=value ;
                RaisePropertyChanged();
            }
        }
        
        /// <summary>
        /// 收听数量
        /// </summary>
        public string ListenerCountString
        {
            get { return _innerModel.ListenerCountString; }
            set { _innerModel.ListenerCountString = value;
                RaisePropertyChanged();
            }
        }
        public string RealUrl => _innerModel.RealUrl;

    }
}
