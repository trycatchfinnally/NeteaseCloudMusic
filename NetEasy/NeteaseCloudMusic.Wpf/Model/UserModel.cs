using NeteaseCloudMusic.Global.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Model
{
   public  class UserModel: BindableBase
   {
       private readonly User _innerModel = new User();
        /// <summary>
        /// 获取或设置用户头像
        /// </summary>
        public string  UserImage
        {
            get { return _innerModel.UserImage; }
            set { _innerModel.UserImage = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 获取或设置用户名
        /// </summary>

        public string  UserName
        {
            get { return _innerModel.UserName; }
            set { _innerModel.UserName = value;
                RaisePropertyChanged();
            }
        }

    }
}
