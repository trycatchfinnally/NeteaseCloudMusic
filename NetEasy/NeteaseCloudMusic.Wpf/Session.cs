using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf
{
    public static class Session
    {
        /// <summary>
        /// 当前登陆的用户
        /// </summary>
        public static Global.Model.User CurrentUser { get; private set; }
        /// <summary>
        /// 通过手机号登陆
        /// </summary>
        /// <param name="cellPhone">手机号</param>
        /// <param name="passWord">密码</param>
        /// <param name="remember"></param>
        public static async Task<string> LogInByCellPhone(string cellPhone, string passWord, bool remember = true)
        {
            var cookieFile = Environment.CurrentDirectory + "/cookies.dat";
            if (File.Exists(cookieFile))
                File.Delete(cookieFile);
            var netWork = CommonServiceLocator.ServiceLocator.Current.GetInstance<Services.NetWork.INetWorkServices>();
            using (var md5 = new MD5CryptoServiceProvider())
            {

                byte[] passWordBytes = Encoding.UTF8.GetBytes(passWord);
                byte[] targetData = md5.ComputeHash(passWordBytes);
                var sb = new StringBuilder();
                for (int i = 0; i < targetData.Length; i++)
                {
                    sb.Append(targetData[i].ToString("x2"));
                }
                passWord = sb.ToString();
            }

            var temp = JsonConvert.DeserializeObject<KeyValuePair<string, Global.Model.User>>(await netWork.PostAsync("Login", "LoginByCellPhone", new { phone = cellPhone, passWord, remember = true }));
            CurrentUser = temp.Value;
            if (temp.Value == null)
            {
               
                return temp.Key;
            }
            var cookie = netWork.Cookie;
            //if (File.Exists(cookieFile))
            {
                using (var stream = File.Create(cookieFile))
                {


                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookie);


                }
                File.SetLastWriteTime(cookieFile, DateTime.Now);
                File.SetAttributes(cookieFile, FileAttributes.ReadOnly);
                File.SetAttributes(cookieFile, FileAttributes.Hidden);
            }
            return "OK";
        }
        public static void LogOut()
        {
            var netWork = CommonServiceLocator.ServiceLocator.Current.GetInstance<Services.NetWork.INetWorkServices>();

        }
        /// <summary>
        /// 刷新登陆，返回登陆成功与否
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RefreshLog( )
        {
            var netWork = CommonServiceLocator.ServiceLocator.Current.GetInstance<Services.NetWork.INetWorkServices>();
            var json = await netWork.GetAsync ("Login", "RefreshLog");
            var data = new { code = 2, message = "" };
            data = JsonConvert.DeserializeAnonymousType(json, data);
            return data.code == 200;
        }
    }
}
