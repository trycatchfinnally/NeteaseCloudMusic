using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.AudioDecode
{
    public interface  IAudioPlayableServices
    {
        /// <summary>
        /// 长度
        /// </summary>
        TimeSpan Length { get; }
        /// <summary>
        /// 当前的播放状态
        /// </summary>
        PlayState PlayState { get; }
        /// <summary>
        /// 音量 0-100
        /// </summary>
        Single Volumn { get; set; }
        /// <summary>
        /// 播放位置
        /// </summary>
        TimeSpan Position { get; set; }
        /// <summary>
        /// 播放指定位置的文件
        /// </summary>
        /// <param name="url">网络地址或者本地文件地址</param>
        void Play(string url);
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 继续播放（暂停后恢复）
        /// </summary>
        void Resume();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
