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
    /// 代表歌手的Model
    /// </summary>
    public class ArtistModel : BindableBase
    {
         
        private readonly Artist _innerModel=new Artist ();
        /// <summary>
        /// 代表歌手名
        /// </summary>
        public string Name
        {
            get
            {
                return _innerModel.Name;
            }

            set
            {
                
                _innerModel.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        /// <summary>
        /// 歌手图片
        /// </summary>
        public string  ArtistImage
        {
            get
            {
                return _innerModel.ArtistImage;
            }

            set { _innerModel.ArtistImage = value;
                RaisePropertyChanged(nameof(ArtistImage));
            }
        }

    }
}
