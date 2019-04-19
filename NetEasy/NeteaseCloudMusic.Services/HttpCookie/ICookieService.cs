using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Services.HttpCookie
{
    public interface ICookieService
    {
        /// <summary>
        /// 保存cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        bool SaveCookie(CookieContainer cookie,User user);
        /// <summary>
        /// 读取cookie
        /// </summary>
        /// <returns></returns>
        (CookieContainer cookie,User user) ReadCookie();
        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <returns></returns>
        bool DeleteCookie();
         
    }
}
