using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 插入表的Model，添加属性未被设置时候不能入表功能
    /// </summary>
  public abstract   class UpdatableDataModelBase
    {
        /// <summary>
        /// 调用set访问器的时候添加进去
        /// </summary>
        protected readonly  List<string> _propDic=new List<string> ();
       
        /// <summary>
        /// 检查某属性是否已经设置值
        /// </summary>
        /// <param name="prop">属性 </param>
        /// <returns></returns>
        internal bool CheckIsSetValue(System.Reflection.PropertyInfo prop )
        {
             
            return _propDic.HasValue(x => x == prop.Name);

            
        }
    }
}
