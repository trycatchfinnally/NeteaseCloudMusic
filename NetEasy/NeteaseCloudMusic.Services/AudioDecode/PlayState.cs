using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.AudioDecode
{
    /// <summary>
    /// 表示播放状态
    /// </summary>
    public enum  PlayState
    {

        UnKnown=-1,
        Stopped=0,
        Playing=1,
        Paused=2
    }
}
