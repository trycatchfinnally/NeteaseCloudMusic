
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
         Task<string > GetAsync(string controllerName, string actionName, object queryStringData = null);
        /// <summary>
        /// 向指定的控制器下的指定行为发送异步get请求
        /// </summary>
        /// <param name="controllerName">服务器中的控制器名称</param>
        /// <param name="actionName">服务器中的行为名</param>
        /// <param name="queryStringData">通过queryString拼接的数据</param>
        /// <param name="cancelToken">有关取消操作的通知</param>
        /// <returns>json形式的数据</returns>
        Task<string> GetAsync(string controllerName, string actionName, object queryStringData ,System.Threading.CancellationToken cancelToken);
        /// <summary>
        /// 将json形式的数据转换成xml格式数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        XDocument Json2Xml(string json);
    }
}
