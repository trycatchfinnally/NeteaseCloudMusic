using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows.Media;
using NeteaseCloudMusic.Global.Model;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.Model
{
    /// <summary>
    /// 代表主播电台目录的绑定类型
    /// </summary>
    public class AnchorRadioCatalogueModel : BindableBase
    {
        private readonly  AnchorRadioCatalogue _innerModel=new AnchorRadioCatalogue ();
        /// <summary>
        /// 代表主播电台目录的名称
        /// </summary>
        public string  CatalogueImage
        {
            get { return _innerModel.PicUrl; }
            set
            {
              
               
                _innerModel.PicUrl = value;
                RaisePropertyChanged(nameof(CatalogueImage));
            }
        }
       
        /// <summary>
        /// 代表主播电台目录的类别名
        /// </summary>
        public string CatalogueTitle
        {
            get { return _innerModel.Name; }
            set
            {
                _innerModel.Name = value;
                RaisePropertyChanged(nameof(CatalogueTitle));
            }
        }

    }
}
