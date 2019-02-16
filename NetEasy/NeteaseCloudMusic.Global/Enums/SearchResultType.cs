namespace NeteaseCloudMusic.Global.Enums
{
    public enum SearchResultType
    {
        /// <summary>
        /// 单曲
        /// </summary>
        Music = 1,
        /// <summary>
        /// 歌手
        /// </summary>
        Artist = 100,
        /// <summary>
        /// 专辑
        /// </summary>
        Album = 10,
        /// <summary>
        /// 歌单
        /// </summary>
        PlayList = 1000,
        /// <summary>
        /// 用户
        /// </summary>
        User = 1002,
        /// <summary>
        /// MV
        /// </summary>
        MV = 1004,
        /// <summary>
        /// 歌词
        /// </summary>
        LRC = 1006,
        /// <summary>
        /// 电台
        /// </summary>
        Radio = 1009,
        /// <summary>
        /// 所有结果都来一份
        /// </summary>
        All = Music | Artist | Album | PlayList | User | MV | Radio
    }
}
