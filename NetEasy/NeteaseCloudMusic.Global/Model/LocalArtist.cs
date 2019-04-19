using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
   public  class LocalArtist
    {
        /// <summary>
        /// 歌手名
        /// </summary>
        public string  Name { get; set; }
        /// <summary>
        /// 歌手图片地址
        /// </summary>
        public string  PicPath { get; set; }
        /// <summary>
        /// 对应的歌曲
        /// </summary>
        public LocalMusic[] LocalMusics { get; set; }

    }
}
