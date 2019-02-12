namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表歌单的model
    /// </summary>
    public class PlayList
    {
        /// <summary>
        /// 歌单的图片
        /// </summary>

        public string PicUrl
        {
            set; get;
        }
        /// <summary>
        /// 用来记录歌单对应的id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 表示歌单标题
        /// </summary>

        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 表示歌单创建者名称
        /// </summary>

        public string CopyWriter
        {
            get; set;
        }

        public double PlayCount
        {
            get; set;
        }
        /// <summary>
        /// 代表歌单的歌曲数目
        /// </summary>
        public int TrackCount { get; set; }
        /// <summary>
        /// 创建改歌单的用户
        /// </summary>
        public User CreateUser { get; set; }
    }
}
