using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;

namespace NeteaseCloudMusic.Wpf.Proxy
{
    /// <summary>
    /// 所有的需要共享的弹出窗体
    /// </summary>
    public sealed class InteractionRequestsProxy
    {
        /// <summary>
        /// 登陆窗口
        /// </summary>
        public InteractionRequest<Confirmation> LoginInteractionRequest { get; } = new InteractionRequest<Confirmation>();
        /// <summary>
        /// 购买窗口
        /// </summary>
        public InteractionRequest<Confirmation> PayMusicInteractionRequest { get; } = new InteractionRequest<Confirmation>();
        /// <summary>
        /// 用来弹出网络连接失败等提示信息的嘿嘿的自动消失的窗体
        /// </summary>
        public InteractionRequest<Notification> AutoDisappearPopupRequest { get; } = new InteractionRequest<Notification>();
        /// <summary>
        /// 等待完成支付弹出窗体
        /// </summary>
        public InteractionRequest<Confirmation> WaitPayRequest { get; } = new InteractionRequest<Confirmation>();
    }
}
