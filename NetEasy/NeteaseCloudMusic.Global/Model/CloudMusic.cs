namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表云盘的音乐
    /// </summary>
    public class CloudMusic
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        /// <summary>
        /// 与之相似的音乐
        /// </summary>
        public Music SimpleMusic { get; set; }

        /// <summary>
        /// 文件大小，字节数
        /// </summary>
        public FileSize FileSize { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string Exetension
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    return string.Empty;
                }

                return System.IO.Path.GetExtension(FileName);
            }
        }
    }
}
