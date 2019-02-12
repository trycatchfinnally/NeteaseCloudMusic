using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NeteaseCloudMusic.Core.Configuration
{
    /// <summary>
    /// 配置信息类
    /// </summary>
  public   class NeteaseCloudMusicConfig: IConfigurationSectionHandler
    {
        /// <summary>
        /// 是否忽略启动任务
        /// </summary>
        public bool IgnoreStartupTasks { get; private set; }
        /// <summary>
        /// api加密Key
        /// </summary>
        public string NeteaseKey { get;private  set; }
        public string NeteasyPubKey { get;private  set; }
        public string NeteasyModulus { get;private  set; }
        public string NeteasyNonce { get; set; }
        private T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default(T);
            var attr = node.Attributes[attrName];
            if (attr == null) return default(T);
            return converter.Invoke(attr.Value);
        }
        private string  SetByXElement (XmlNode node, string attrName,  string defaultValue)
        {
            var temp = SetByXElement(node, attrName, Convert.ToString);
            if (string.IsNullOrEmpty(temp))
                return defaultValue;
            return temp;
        }
        /// <summary>
        /// 创建配置节处理程序。
        /// </summary>
        /// <param name="parent">父元素</param>
        /// <param name="configContext">配置上下文</param>
        /// <param name="section">选择的XML节点</param>
        /// <returns>创建的部分处理程序对象</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new NeteaseCloudMusicConfig();
            var startupNode = section.SelectSingleNode("Startup");
            config.IgnoreStartupTasks = SetByXElement(startupNode, "IgnoreStartupTasks", Boolean.Parse);
            var keyNode = section.SelectSingleNode("Keys");
            config.NeteaseKey = SetByXElement(keyNode, nameof(NeteaseKey), "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
            config.NeteasyPubKey = SetByXElement(keyNode, nameof(NeteasyPubKey), "010001");
            config.NeteasyModulus = SetByXElement(keyNode, nameof(NeteasyModulus), "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7");
            config.NeteasyNonce = SetByXElement(keyNode, nameof(NeteasyNonce), "0CoJUm6Qyw8W8jud");
           
            return config;
        }

    }
}
