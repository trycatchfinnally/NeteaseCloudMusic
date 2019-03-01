using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// 所有的需要共享的弹出窗体
    /// </summary>
  public static  class InteractionRequests
    {
        /// <summary>
        /// 表示登陆弹出窗口
        /// </summary>
        public static InteractionRequest<Confirmation> LoginInteractionRequest { get; } = new InteractionRequest<Confirmation>();
        /// <summary>
        /// 表示购买窗口
        /// </summary>
        public static InteractionRequest<Confirmation> PayMusicInteractionRequest { get; } = new InteractionRequest<Confirmation>();
    }
}
