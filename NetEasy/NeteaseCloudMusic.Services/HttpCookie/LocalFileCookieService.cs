using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Services.Properties;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeteaseCloudMusic.Services.HttpCookie
{
    public class LocalFileCookieService : ICookieService
    {
        [Serializable]
        private class LocalDiskModel
        {
            public CookieContainer Cookie { get; set; }
            public User User { get; set; }
        }

       // private readonly INetWorkServices _netWorkServices;
        public LocalFileCookieService()
        {
           
        }
        private readonly string _cookieFilePath = Path.Combine(Environment.CurrentDirectory, "cookie.dat");
        public bool SaveCookie(CookieContainer cookie, User user)
        {
            if (cookie == null || user == null)
            {
                throw new ArgumentNullException();
            }
            if (File.Exists(this._cookieFilePath))
            {
                File.Delete(this._cookieFilePath);
            }
            var data = new LocalDiskModel() { User = user, Cookie = cookie };
            using (var stream = File.OpenWrite(this._cookieFilePath))
            {

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
            File.SetLastWriteTime(this._cookieFilePath, DateTime.Now);
            File.SetAttributes(this._cookieFilePath, FileAttributes.ReadOnly);
            File.SetAttributes(this._cookieFilePath, FileAttributes.Hidden);
            return true;
        }

        public (CookieContainer cookie, User user) ReadCookie()
        {
            if (!File.Exists(this._cookieFilePath))
            {
                return default(ValueTuple<CookieContainer, User>);
            }


            using (var fs = File.OpenRead(this._cookieFilePath))
            {
                var formatter = new BinaryFormatter();
                var tmp = formatter.Deserialize(fs) as LocalDiskModel;
                if (tmp == null)
                {
                    throw new FormatException();
                }
                var expired = tmp.Cookie.GetCookies(new Uri(Resources.ServiceUri)).Cast<Cookie>().Any(x => x.Expired);
                if (expired)
                {
                    DeleteCookie();
                    return default(ValueTuple<CookieContainer, User>);

                }

                return (tmp.Cookie, tmp.User);
            }



        }

        public bool DeleteCookie()
        {
            if (File.Exists(this._cookieFilePath))
            {
                File.Delete(this._cookieFilePath);
            }

            return true;
        }
    }
}
