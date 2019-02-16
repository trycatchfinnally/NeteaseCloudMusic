namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表电台的model
    /// </summary>
    public class Radio
    {
        /// <summary>
        /// radio图片
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 电台的标题
        /// </summary>
        public string Title { get; set; }
      
        /// <summary>
        /// 表示主播名
        /// </summary>
        public string CopyWriter
        {
            get;set;
        }
        
        /// <summary>
        /// 电台的描述
        /// </summary>
        public string Description
        {
            set;get;
        }
        /// <summary>
        /// 电台的订阅数量
        /// </summary>
        public int SubscribedCount
        {
            set;get;
        }
        public long  Id { get; set; }

    }
}