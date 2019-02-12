using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Wpf.Model
{
   public  class MenuListItemModel: BindableBase
    {
         
        private readonly MenuListItem _innerModel = new MenuListItem();
        /// <summary>
        /// 每一项对应的图标
        /// </summary>
        public String ItemIcon
        {
            get { return _innerModel.ItemIcon; }
            set { _innerModel.ItemIcon = value;
                RaisePropertyChanged(nameof(ItemIcon));
            }
        }
        
        /// <summary>
        /// 每一项对应显示的文字
        /// </summary>
        public string  Text
        {
            get { return _innerModel.Text; }
            set {_innerModel.Text=value;
                RaisePropertyChanged(nameof(Text));
            }
        }

    }
}
