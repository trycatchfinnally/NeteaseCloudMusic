

using Newtonsoft.Json;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using NeteaseCloudMusic.Services.HttpCookie;
using NeteaseCloudMusic.Services.Properties;

namespace NeteaseCloudMusic.Services.NetWork
{

    public class NeteaseCloundMusicNetWorkServices : INetWorkServices 
    {
        private readonly ICookieService _cookieService;

        /// <summary>
        /// 服务器的基础地址
        /// </summary>
      //private const string ServicesBaseUrl = "http://localhost:10086/api/";
     private readonly  string ServicesBaseUrl = $"{Resources.ServiceUri}/api/";
      //  public string ServicesBaseUrl { get; }= "http://47.101.43.134/api/";
        private readonly HttpClient _httpClient;
        private readonly HttpClientHandler _httpClientHandler;
        public NeteaseCloundMusicNetWorkServices(ICookieService cookieService)
        {
            this._cookieService = cookieService;
            var (cookieContainer, user) = cookieService.ReadCookie();
            this._httpClientHandler = new HttpClientHandler { UseCookies = true  };
            if (cookieContainer != null) this._httpClientHandler.CookieContainer = cookieContainer;
             this._httpClient = new HttpClient(this._httpClientHandler);
             
        }
        public CookieContainer Cookie
        {
            get { return this._httpClientHandler.CookieContainer; }
            set { this._httpClientHandler.CookieContainer = value ; }
        }

        public async Task<NetWorkDataResult<T>> GetAnonymousTypeAsync<T>(string controllerName, string actionName, object queryStringData, T anonymousTypeObject)  
        {
            return await GetAsync<T>(controllerName, actionName, queryStringData, CancellationToken.None);
        }

        public async Task<NetWorkDataResult<T>> GetAsync<T>(string controllerName, string actionName, object queryStringData = null)
        {
            return await GetAsync<T>(controllerName, actionName, queryStringData, CancellationToken.None);
        }
 
        public async Task<NetWorkDataResult<T>> GetAsync<T>(string controllerName, string actionName, object queryStringData, CancellationToken cancelToken)
        {

            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
                throw new ArgumentException("控制器名或者行为名不合法！");
            StringBuilder urlSb = new StringBuilder(ServicesBaseUrl);
            urlSb.Append($"{controllerName}/{actionName}");
            if (queryStringData != null)
            {
                var props = queryStringData.GetType().GetProperties();
                if (props.Length == 0)
                {
                    throw new ArgumentException("附带的参数无法读取项目");
                }
                urlSb.Append($"?{props[0].Name}={props[0].GetValue(queryStringData)}");
                for (int i = 1; i < props.Length; i++)
                {
                    urlSb.Append($"&{props[i].Name}={props[i].GetValue(queryStringData)}");
                }
            }

            //  HttpClient hClient = new HttpClient(_httpClientHandler) { Timeout = TimeSpan.FromSeconds(90) };
            var result = new NetWorkDataResult<T>();
            try
            {

                var tmp = await this._httpClient.GetAsync(urlSb.ToString(), cancelToken);
                if (tmp.RequestMessage.Headers.Contains("Cookies")|| tmp.RequestMessage.Headers.Contains("Set-Cookie")) {
                    Console.WriteLine(tmp.RequestMessage.Headers);
                }
               // tmp.Wait();
                tmp.EnsureSuccessStatusCode();
                cancelToken.ThrowIfCancellationRequested();
                string json= await tmp.Content.ReadAsStringAsync();
                result.Successed = true;
                if (typeof(T) == typeof(string))
                    result.Data =(T)(object)json;
                else
                result.Data = JsonConvert.DeserializeObject<T>(json);
            }
            catch(System.Net.Http.HttpRequestException exception)
            {
                result.Successed = false;
                result.ErrorMessage = exception.Message;
            }
            return result;
            

        }
        //public XDocument Json2Xml(string json)
        //{
        //    if (string.IsNullOrEmpty(json))
        //        throw new ArgumentNullException(nameof(json));
        //    return JsonConvert.DeserializeXNode(json,"Root");
        //}

        public async Task<string> PostAsync(string controllerName, string actionName, object postData)
        {
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
                throw new ArgumentException("控制器名或者行为名不合法！");
            HttpContent requestContent = new FormUrlEncodedContent(postData.GetType().GetProperties().Select(x=>new KeyValuePair<string, string>(x.Name,x.GetValue(postData).ToString())) );
            var temp = await this._httpClient.PostAsync(ServicesBaseUrl + controllerName + "/" + actionName, requestContent);
            temp.EnsureSuccessStatusCode();
            return await temp.Content.ReadAsStringAsync();
        }

     
    }
}
