using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Wpf.Model;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf
{
    public static class Context
    {
        /// <summary>
        /// 当前的区域名
        /// </summary>
        public const string RegionName = "MainRegion";
        public const string SupportFileExtension = "*.mp3|*.flac|*.cs";
        /// <summary>
        /// 每页请求的数据限制
        /// </summary>
        public const int LimitPerPage = 30;
        /// <summary>
        /// 当前正在播放的歌曲列表
        /// </summary>
        public static ObservableCollection<Music> CurrentPlayMusics { get; } = new ObservableCollection<Music>();
        /// <summary>
        /// 表示播放的命令 
        /// </summary>
        public static CompositeCommand PlayCommand { get; } = new CompositeCommand();
        /// <summary>
        /// 表示暂停的命令
        /// </summary>
        public static CompositeCommand PauseCommand { get; } = new CompositeCommand();
        /// <summary>
        /// 表示下一个的命令
        /// </summary>
        public static CompositeCommand NextTrackCommand { get; } = new CompositeCommand();
        /// <summary>
        /// 代表上一个的命令
        /// </summary>
        public static CompositeCommand PrevTrackCommand { get; } = new CompositeCommand();
    }
    /// <summary>
    /// 当当前播放音乐发生变化的聚合事件参数
    /// </summary>
    public class CurrentPlayMusicChangeEvent: PubSubEvent<Music>
    {

    }
}
