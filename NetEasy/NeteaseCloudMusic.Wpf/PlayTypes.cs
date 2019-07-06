using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// 表示播放器当前播放的类型
    /// </summary>
    public enum PlayTypes
    {
        /// <summary>
        /// 播放的本地音乐
        /// </summary>
        Local,
        /// <summary>
        /// 在线音乐
        /// </summary>
        Online,
        /// <summary>
        /// 播放MV
        /// </summary>
        Mv,
        /// <summary>
        /// 电台
        /// </summary>
        Radio,
        /// <summary>
        /// 私人FM
        /// </summary>
        PersonalFm,
    }
}
