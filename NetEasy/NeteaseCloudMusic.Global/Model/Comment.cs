using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表一条评论
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// 评论的id
        /// </summary>
        public long CommentId { get; set; }
        /// <summary>
        /// 评论的内容
        /// </summary>
        public string Content { get; set; }

        public bool IsRemoveHotComment { get; set; }

        public int LikedCount { get; set; }
        /// <summary>
        /// 评论的时间
        /// </summary>
        public DateTime Time { get; set; }

        public int Status { get; set; }
        /// <summary>
        /// 评论的用户
        /// </summary>
        public User User { get; set; }
       // public List<Comment> BeReplied { get; set; }

        public bool Liked { get; set; }
        /// <summary>
        /// 表示回复的某人的信息
        /// </summary>
        public List<Comment> BeReplied { get; set; }
        /// <summary>
        /// 是否有恢复某人信息
        /// </summary>
        public bool HasReplied => (BeReplied?.Count).GetValueOrDefault() > 0;
    }
}
