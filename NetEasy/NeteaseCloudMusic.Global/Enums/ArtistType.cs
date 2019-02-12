namespace NeteaseCloudMusic.Global.Enums
{
    /// <summary>
    /// 歌手类型分分类
    /// </summary>
  public   enum ArtistType
    {
        /// <summary>
        /// 男歌手
        /// </summary>
        Man=0,
        /// <summary>
        /// 女歌手
        /// </summary>
        Woman=1,
        /// <summary>
        /// 乐队组合
        /// </summary>
        Band=2,
        /// <summary>
        /// 全部
        /// </summary>
        All=Man|Woman|Band
    }
}
