using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.NetWork
{
   public abstract class NetWorkDataResultBase
    {
        /// <summary>
        /// 返回结果是否成功
        /// </summary>
        public bool Successed { get;protected internal set; }
    }
}
