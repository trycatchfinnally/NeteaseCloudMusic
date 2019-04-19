using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
  public   class LocalMusic
    {
        /// <summary>
        /// 每一个本地音乐的特定标识符
        /// </summary>
        public long Id
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    throw new ArgumentException();
                return FilePath.GetHashCode();
            }
        }
        public string  FilePath { get; set; }
        /// <summary>
        /// 本地音乐文件的标题
        /// </summary>
        public string  Title { get; set; }
        /// <summary>
        /// 本地音乐的时长
        /// </summary>
        public TimeSpan Duration { get; set; }
        /// <summary>
        /// 本地音乐的文件大小
        /// </summary>
        public FileSize FileSize { get; set; }
        /// <summary>
        /// 本地音乐文件的专辑
        /// </summary>
        public string AlbumName { get; set; }
        /// <summary>
        /// 本地音乐的歌手名称
        /// </summary>
        public string[] ArtistsName { get; set; }

        public string Id3Pic { get; set; }
        /// <summary>
        /// 相似的在线音乐
        /// </summary>
        public Music SimiOnlineMusic { get; set; }
    }
}
