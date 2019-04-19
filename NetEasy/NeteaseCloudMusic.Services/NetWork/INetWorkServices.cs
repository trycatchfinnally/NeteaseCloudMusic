
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NeteaseCloudMusic.Services.NetWork
{
    /// <summary>
    /// 请求网络服务
    /// </summary>
    public interface  INetWorkServices
    {

        /// <summary>
        /// 向指定的控制器下的指定行为发送异步get请求
        /// </summary>
        /// <param name="controllerName">服务器中的控制器名称</param>
        /// <param name="actionName">服务器中的行为名</param>
        /// <param name="queryStringData">通过queryString拼接的数据</param>
        /// <returns>json形式的数据</returns>
        Task<NetWorkDataResult<T>> GetAsync<T>(string controllerName, string actionName, object queryStringData = null);
        /// <summary>
        /// 向指定的控制器下的指定行为发送异步get请求
        /// </summary>
        /// <param name="controllerName">服务器中的控制器名称</param>
        /// <param name="actionName">服务器中的行为名</param>
        /// <param name="queryStringData">通过queryString拼接的数据</param>
        /// <param name="cancelToken">有关取消操作的通知</param>
        /// <returns>json形式的数据</returns>
        Task<NetWorkDataResult<T>> GetAsync<T>(string controllerName, string actionName, object queryStringData ,System.Threading.CancellationToken cancelToken);
        Task<NetWorkDataResult<T>> GetAnonymousTypeAsync<T>(string controllerName, string actionName, object queryStringData, T anonymousTypeObject);
        /// <summary>
        /// 用于登陆用，发送post请求
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        Task<string> PostAsync(string controllerName, string actionName, object postData);
        ///// <summary>
        ///// 将json形式的数据转换成xml格式数据
        ///// </summary>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //XDocument Json2Xml(string json);
        /// <summary>
        /// 获取或设置用于通过处理程序存储服务器 Cookie 的 Cookie 容器。
        /// </summary>
        System.Net.CookieContainer Cookie { get; set; }
    }
}
