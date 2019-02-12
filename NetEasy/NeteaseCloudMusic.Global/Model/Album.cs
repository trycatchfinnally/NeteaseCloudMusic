using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
  public   class Album
    {
        public long Id { get; set; }
        public string  PicUrl { get; set; }
        public string Name   { get; set; }
        /// <summary>
        /// 发行时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public List<Music> Musics { get; set; }
        public List<Artist> Artists { get; set; }
        /// <summary>
        /// 收藏数目
        /// </summary>
        public int CollectionCount { set; get; }
        /// <summary>
        /// 评论数目
        /// </summary>
        public int CommentCount { get; set; }
    }
}
