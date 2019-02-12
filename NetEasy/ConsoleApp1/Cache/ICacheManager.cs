using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Cache
{
    /// <summary>
    /// 代表缓存管理器
    /// </summary>
    public interface  ICacheManager:IDisposable
    {
        /// <summary>
        /// 通过key获取值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">值</param>
        /// <param name="cacheTime">缓存时间</param>
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        /// <summary>
        /// 根据key删除项目
        /// </summary>
        /// <param name="pattern"></param>
        void Remove(string key);
        /// <summary>
        /// 按匹配字符串删除项目
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveByPattern(string pattern);
        /// <summary>
        /// 清空缓存管理器
        /// </summary>
        void Clear();
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// 获取缓存项目。 如果它还没有在缓存中，则加载并缓存它
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存的key值</param>
        /// <param name="cacheTime">以分钟表示的缓存时间（0-没有缓存）</param>
        /// <param name="acquire">如果尚未在缓存中加载项目执行的方法</param>
        /// <returns></returns>
        public static  T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (cacheManager.IsSet(key))
                return cacheManager.Get<T>(key);
            var result = acquire();
            if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);
            return result;
        }
        /// <summary>
        /// 获取缓存项目。 如果它还没有在缓存中，则加载并缓存它
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存的key值</param>
        /// <param name="acquire">如果尚未在缓存中加载项目执行的方法</param>
        /// <returns></returns>
        public static  T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire) => cacheManager.Get(key, 60, acquire);
        public static  void RemoveByPattern(this ICacheManager cacheManager, string pattern, IEnumerable<string> keys)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            keys.Where(p => regex.IsMatch(p.ToString())).Each(x => cacheManager.Remove(x));
        }
    }
}
