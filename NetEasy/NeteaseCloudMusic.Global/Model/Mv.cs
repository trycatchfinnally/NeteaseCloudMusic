using System.Linq;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表MV的绑定Model
    /// </summary>
    public class Mv
    {
        /// <summary>
        /// mv的id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// mv图片
        /// </summary>
        public string PicUrl
        {
            get; set;
        }
        /// <summary>
        /// mv的时长
        /// </summary>
        public System.TimeSpan Duration { get; set; }
        /// <summary>
        /// mv的标题
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 表示歌手名
        /// </summary>
        public string ArtistName
        {
            get
            {
                if (Artists == null || Artists.Length == 0)
                    return string.Empty;
                return string.Join("/", Artists.Select(x => x.Name));
            }
        }
        public Artist[] Artists { get; set; }
        /// <summary>
        /// 作者的id
        /// </summary>
        public long ArtistId => Artists?.FirstOrDefault()?.Id??0;
        /// <summary>
        /// mv被播放次数
        /// </summary>
        public long PlayCount
        {
            get; set;
        }
        /// <summary>
        /// mv的描述
        /// </summary>
        
        public string Desc
        {
            get; set;
        }
        /// <summary>
        ///  mv的地址,包括分辨率和地址
        /// </summary>
        public System.Collections.Generic.Dictionary<int,string>  Url { get; set; }
        /// <summary>
        /// 收藏数
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 评论的索引
        /// </summary>
        public string  CommendThreadId { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public System.DateTime PublishTime { get; set; }
    }
}
