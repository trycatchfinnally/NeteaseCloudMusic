using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
  public   class CloudDisk
    {
        public int Count { get; set; }
        /// <summary>
        /// 已使用的容量
        /// </summary>
        public FileSize Size { get; set; }
        /// <summary>
        /// 云盘总容量
        /// </summary>
        public FileSize MaxSize { get; set; }
        /// <summary>
        /// 云盘是否还有更多,分页用
        /// </summary>
        public bool  HasMore { get; set; }
        /// <summary>
        /// 当前页的数据
        /// </summary>
        public CloudMusic[] CloudMusics { get; set; }

    }
}
