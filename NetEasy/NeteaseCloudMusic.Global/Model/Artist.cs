using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表歌手的Model
    /// </summary>
    public class Artist  
    {

        public long  Id { get; set; }
        /// <summary>
        /// 代表歌手名
        /// </summary>
        public string Name
        {
            get;set;
        }
        /// <summary>
        /// 歌手图片
        /// </summary>
       [JsonIgnore]
        public string ArtistImage
        {
            get;set;
        }
        /// <summary>
        /// 歌手图片，遗留的就不改了
        /// </summary>
        public string PicUrl
        {
            get { return ArtistImage; }
            set { ArtistImage = value; }
        }
        public List<Music>   HotMusics { get; set; }
        /// <summary>
        /// mv数量
        /// </summary>
        public int MvCount { get; set; }
        /// <summary>
        /// 音乐数量
        /// </summary>
        public int MusicCount { get; set; }
        /// <summary>
        /// 专辑数量
        /// </summary>
        public int AlbumCount { get; set; }
        /// <summary>
        /// 热度
        /// </summary>
        public int  HotScore { get; set; }
        /// <summary>
        /// 如果歌手有账户，对应账户id
        /// </summary>
        public long  AccountId { get; set; }
    }

}
