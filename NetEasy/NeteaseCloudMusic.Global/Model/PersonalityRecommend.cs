using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
    /// <summary>
    /// 个性推荐对应的model
    /// </summary>
  public   class PersonalityRecommend
    {
       
        ///// <summary>
        ///// 代表轮播图
        ///// </summary>
        //public class Banner
        //{
        //    public string  Pic { get; set; }
        //}
        /// <summary>
        /// 推荐列表
        /// </summary>
        public List<PlayList> RecommendList { get; set; }
        /// <summary>
        /// 获取或设置轮播图
        /// </summary>
        public List<Banner> BannerList { get; set; }
        /// <summary>
        /// 最新音乐
        /// </summary>
        public List<Music> NewMusicList { get; set; }
        /// <summary>
        /// 电台
        /// </summary>
        public List<Radio> AnchorRadioList { get; set; }
        /// <summary>
        /// 推荐MV
        /// </summary>
        public List<Mv> RecommendMvList { get; set; }
        /// <summary>
        /// 独家放送
        /// </summary>
        public List<PictureListBoxItem> PrivateContentList { get; set; }

    }
}
