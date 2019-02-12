namespace NeteaseCloudMusic.Global.Model
{
    public class PictureListBoxItem 
    {
        /// <summary>
        /// 获取或设置显示的图片
        /// </summary>
        public string ImageSource
        {
            set;get;

        }
        /// <summary>
        /// 获取或设置显示的项描述
        /// </summary>
        public string Text
        {
            get;set;
        }
        /// <summary>
        /// 收听数量
        /// </summary>
        public string ListenerCountString
        {
            set;get;
        }
        /// <summary>
        /// 连接地址
        /// </summary>
        public string  RealUrl { get; set; }

    }
}
