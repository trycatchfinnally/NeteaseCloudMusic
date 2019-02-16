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
    /// 轮播图对应的绑定Model
    /// </summary>
 public    class BannerModel: BindableBase
    {
        private readonly  Banner _innerModel = new Banner();
        /// <summary>
        /// 对应的图片
        /// </summary>
        public string  Image
        {
            get { return _innerModel.PicUrl; }
            set { _innerModel.PicUrl = value;
                RaisePropertyChanged(nameof(Image));
            }
        }

    }
}
