using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 指定更新字段选项
    /// </summary>
    public enum  UpdateFieldsOptions
    {
        /// <summary>
        /// 排除指定的字段
        /// </summary>
        ExcludeFields = 1,
        /// <summary>
        /// 包含指定的字段
        /// </summary>
        IncludeFields = 0

    }
}
