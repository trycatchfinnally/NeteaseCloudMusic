using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
   public  class PlayListDetail
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        public string  PicUrl { get; set; }
        /// <summary>
        /// 表示歌单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public User CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 收藏数目
        /// </summary>
        public int CollectionCount { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public List<string > Tags { get; set; }
        /// <summary>
        /// 对应的描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 对应的播放列表
        /// </summary>
        public List<Music> Musics { get; set; }
    }
}
