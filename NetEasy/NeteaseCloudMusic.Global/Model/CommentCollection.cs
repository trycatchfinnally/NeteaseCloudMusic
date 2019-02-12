using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表请求返回的评论集合
    /// </summary>
   public  class CommentCollection
    {
        /// <summary>
        /// 分页搜索是否有新的页
        /// </summary>
        public bool More { get; set; }
        /// <summary>
        /// 是否还有更多的热门评论
        /// </summary>
        public bool MoreHot { get; set; }
       /// <summary>
       /// 评论的总数
       /// </summary>
        public int Total { get; set; }
       /// <summary>
       /// 
       /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 查询得到的评论
        /// </summary>
        public List<Comment> Comments { get; set; }
        /// <summary>
        /// 查询得到的热们评论
        /// </summary>
        public List<Comment> HotComments { get; set; }
        /// <summary>
        /// 查询得到的置顶评论
        /// </summary>
        public List<Comment> TopicComments { get; set; }
    }
}
