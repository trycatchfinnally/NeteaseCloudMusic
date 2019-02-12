using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
   public  class BillBoard
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string  PicUrl { get; set; }
        public string UpdateFrequency { get; set; }
        public DateTime CreateTime { get; set; }
        
        public int PlayCount { get; set; }
        /// <summary>
        /// 歌单部分音乐的名称
        /// </summary>
        public List<string > SomeTracksName { get; set; }
    }
}
