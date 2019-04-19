using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Services.NetWork;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NeteaseCloudMusic.Services.HttpCookie;
using System;

namespace NeteaseCloudMusic.Services.Identity
{
    public class DefaultIdentityService : IdentityService
    {
        private readonly INetWorkServices _netWorkServices;
        private readonly ICookieService _cookieService;

        public event EventHandler<bool> LoginStateChanged;

        public DefaultIdentityService(INetWorkServices netWorkServices,ICookieService cookieService)
        {
            this._netWorkServices = netWorkServices;
            this._cookieService = cookieService;
            var tmp = cookieService.ReadCookie();
            CurrentUser = tmp.user;
        }

        private async Task<string> LoginInByCellPhoneAsync(string cellPhoneOrEmail, string passWord, bool remembe)
        {
            var netWork = this._netWorkServices;
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

            var temp = JsonConvert.DeserializeObject<KeyValuePair<string, Global.Model.User>>(await netWork.PostAsync("Login", "LoginByCellPhone", new { phone = cellPhoneOrEmail, passWord, remember = remembe }));
            CurrentUser = temp.Value;
            if (temp.Value == null)
            {
                //IsLoginIn = false;
                return temp.Key;
            }
            //var cookie = netWork.Cookie;
            //SaveCookie(CookieFile, cookie);
            //WriteToFile(UserFile, JsonConvert.SerializeObject(temp.Value));
           // IsLoginIn = true;
            //LoginStateChanged?.Invoke(null, true);
            return "OK";
        }

        private async Task<string> LoginByEmailAsync(string email, string passWord, bool remember = true)
        {
            var netWork = this._netWorkServices;
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

            var temp = JsonConvert.DeserializeObject<KeyValuePair<string, Global.Model.User>>(await netWork.PostAsync("Login", "LoginByEmail", new { email, passWord, remember = true }));
            CurrentUser = temp.Value;
            if (temp.Value == null)
            {
               // IsLoginIn = false;
                return temp.Key;
            }
            var cookie = netWork.Cookie;
            //SaveCookie(CookieFile, cookie);
            //WriteToFile(UserFile, JsonConvert.SerializeObject(temp.Value));
            //IsLoginIn = true;
            //   LoginStateChanged?.Invoke(null, true);
            return "OK";
        }
        public async Task<string> LoginInAsync(string cellPhoneOrEmail, string passWord, bool remember)
        {
            string result;
            if (Regex.IsMatch(cellPhoneOrEmail, @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$"))
                result = await LoginByEmailAsync(cellPhoneOrEmail, passWord, remember);
            else
            {
                result = await LoginInByCellPhoneAsync(cellPhoneOrEmail, passWord, remember);
            }
            if (this.CurrentUser == null) return result;
            var cookie = this._netWorkServices.Cookie;
            this._cookieService.SaveCookie(cookie, this.CurrentUser);
            this.LoginStateChanged?.Invoke(this, true);

            return result;
        }

        public async Task<bool> LoginOutAsync()
        {
            await Task.Delay(100);
            this._cookieService.DeleteCookie();
            //IsLoginIn = false;
            CurrentUser = null;
            this.LoginStateChanged?.Invoke(this, false);

            return false;
        }

        public async Task<bool> RefreshLoginAsync()
        {
            var netWork = this._netWorkServices;
            var data = new { code = 2, message = "" };
            var dataResult = await netWork.GetAnonymousTypeAsync("Login", "RefreshLog", null, data);
            data = dataResult.Data;
            //SaveCookie(CookieFile, netWork.Cookie);
            if (dataResult.Successed && data.code == 200)
            {

                var (cookie, user) = this._cookieService.ReadCookie();
               // this.IsLoginIn = true;
                this.CurrentUser = user;
                return true;
            }
            return false;
        }

        public User CurrentUser { get; private set; }
       // public bool IsLoginIn { get; private set; }
    }
}
