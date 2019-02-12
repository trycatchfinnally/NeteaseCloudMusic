using System;
using System.Collections.Generic;
using System.Linq;
using NeteaseCloudMusic.Global.Enums;
using Newtonsoft.Json;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 代表音乐的绑定Model
    /// </summary>
    public class Music 
    {


        /// <summary>
        /// 热度
        /// </summary>
        public string  Pop { get; set; }
        public string ArtistName { get
            {
                return string.Join(",", this.Artists?.Select(y => y.Name).ToArray() ?? Array.Empty<string>());
            }
        }

        /// <summary>
        /// 歌曲的持续时间
        /// </summary>
        public TimeSpan Duration { get; set; }
      

        /// <summary>
        /// 对应的图片
        /// </summary>
        public string PicUrl
        {
            get;set;
        }
        /// <summary>
        /// 歌曲的标题
        /// </summary>
        public string Name
        {
            get;set;
        }
        /// <summary>
        /// 音乐的质量级别
        /// </summary>
        public MusicQualityLevel MusicQuality
        {
            get; set;
        }
        /// <summary>
        /// 是否独家
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// 歌曲的id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 表示对应的歌手
        /// </summary>
        public Artist Artist { get; set; } = new Artist();
        /// <summary>
        /// 表示多个歌手
        /// </summary>
        public List<Artist>  Artists { get; set; }
        /// <summary>
        /// 代表对应的专辑
        /// </summary>
        public Album Album   { get; set; }
        /// <summary>
        /// 歌曲是否有MV
        /// </summary>
        public bool HasMv => MvId != 0;
         
        /// <summary>
        /// mv地址
        /// </summary>
        public long MvId { get; set; }
     
         
        /// <summary>
        /// 是否喜欢歌曲
        /// </summary>
        public bool IsLike
        {
            get;set;
        }
        /// <summary>
        /// 专辑名
        /// </summary>
        public string AlbumName => this.Album?.Name;
         
        /// <summary>
        /// 歌曲的地址
        /// </summary>
        public string  Url { get; set; }
    }

    public class MusicInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int Br
        {
            get { return BitRate; }
            set { BitRate = value; }
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int BitRate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long Fid { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long Size { get; set; }
        public string Vd { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long DfsId { get; set; }
        public long PlayTime { get; set; }
        [JsonProperty("Sr", NullValueHandling = NullValueHandling.Ignore)]
        public int SampleRate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float VolumeDelta { get; set; }
    }
}
