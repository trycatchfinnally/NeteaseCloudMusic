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
        private static  readonly string CookieFile = Environment.CurrentDirectory + "/cookies.dat";
        private static readonly string UserFile= Environment.CurrentDirectory + "/User.json";
        /// <summary>
        /// 当登陆状态发生变化时候引发的事件
        /// </summary>
        public static event EventHandler<bool> LoginStateChanged;
        /// <summary>
        /// 当前登陆的用户
        /// </summary>
        public static Global.Model.User CurrentUser { get; private set; }
        public static bool IsLoginIn { get; private set; }
        /// <summary>
        /// 通过手机号登陆
        /// </summary>
        /// <param name="cellPhone">手机号</param>
        /// <param name="passWord">密码</param>
        /// <param name="remember"></param>
        public static async Task<string> LogInByCellPhone(string cellPhone, string passWord, bool remember = true)
        {
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
                IsLoginIn = false;
                return temp.Key;
            }
            var cookie = netWork.Cookie;
            SaveCookie(CookieFile, cookie);
            WriteToFile(UserFile, JsonConvert.SerializeObject(temp.Value));
            IsLoginIn = true;
            LoginStateChanged?.Invoke(null, true);
            return "OK";
        }
        public static async Task<string >LoginByEmail(string email, string passWord, bool remember = true)
        {
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

            var temp = JsonConvert.DeserializeObject<KeyValuePair<string, Global.Model.User>>(await netWork.PostAsync("Login", "LoginByEmail", new {  email, passWord, remember = true }));
            CurrentUser = temp.Value;
            if (temp.Value == null)
            {
                IsLoginIn = false;
                return temp.Key;
            }
            var cookie = netWork.Cookie;
            SaveCookie(CookieFile, cookie);
            WriteToFile(UserFile, JsonConvert.SerializeObject(temp.Value));
            IsLoginIn = true;
            LoginStateChanged?.Invoke(null, true);
            return "OK";
        }
        private static void WriteToFile(string filePath,string content)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath,content);
            File.SetLastWriteTime(filePath, DateTime.Now);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
            File.SetAttributes(filePath, FileAttributes.Hidden);
        }
        private static void SaveCookie(string filePath,object cookie)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (var stream = File.OpenWrite(filePath))
            {

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookie);


            }
            File.SetLastWriteTime(filePath, DateTime.Now);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
            File.SetAttributes(filePath, FileAttributes.Hidden);

        }
        public static void LogOut()
        {
            var netWork = CommonServiceLocator.ServiceLocator.Current.GetInstance<Services.NetWork.INetWorkServices>();
            IsLoginIn = false;
            LoginStateChanged?.Invoke(null, false );

        }
        /// <summary>
        /// 刷新登陆，返回登陆成功与否
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RefreshLog()
        {
            var netWork = CommonServiceLocator.ServiceLocator.Current.GetInstance<Services.NetWork.INetWorkServices>();
            var json = await netWork.GetAsync("Login", "RefreshLog");
            var data = new { code = 2, message = "" };
            data = JsonConvert.DeserializeAnonymousType(json, data);
            SaveCookie(CookieFile, netWork.Cookie);
            if (data.code==200)
            {
               

                if (File.Exists(UserFile))
                {
                    try
                    {
                        Session.CurrentUser = JsonConvert.DeserializeObject<Global.Model.User>(File.ReadAllText(UserFile));
                        IsLoginIn = true;
                        LoginStateChanged?.Invoke(null, true);

                        return true;
                    }
                    catch  
                    {

                        return false;
                    }
                }
                return false;
            }
            return false ;
        }
    }
}
