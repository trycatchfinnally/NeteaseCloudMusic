using System.ComponentModel;

namespace NeteaseCloudMusic.Global.Enums
{
    public enum PlayCycleType
    {
        /// <summary>
        /// 全部重复
        /// </summary>
        [DescriptionAttribute("列表循环")]
        RepeatAll = 0,
        /// <summary>
        /// 单曲重复
        /// </summary>
        [DescriptionAttribute("单曲循环")]
        RepeatOne = 1,
        /// <summary>
        /// 顺序播放
        /// </summary>
        [DescriptionAttribute("顺序播放")]
        Order = 2,
        /// <summary>
        /// 随机播放
        /// </summary>
        [DescriptionAttribute("随机播放")]
        Random = 3

    }
}
