using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.UniqueSymbol
{
    public interface  IUniqueServices
    {
        /// <summary>
        /// 获取机器唯一标识符
        /// </summary>
        string UniqueString { get; }
        /// <summary>
        /// 获取0-maxValue之间固定的随机数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        int UniqueNum(int maxValue);
        
    }
}
