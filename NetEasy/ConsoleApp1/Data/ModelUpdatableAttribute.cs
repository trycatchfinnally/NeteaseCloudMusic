using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 用于标识直接更新数据库用的 Data Model 类中的属性与数据库字段的映射关系。
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Property, Inherited = true), System.Runtime.InteropServices.ComVisible(true)]
    public class ModelUpdatableAttribute:Attribute
    {
        /// <summary>
        ///   是否可为空字段
        /// </summary>
        public bool Nullable { get; set; } = true;
        /// <summary>
        /// 映射到数据库中的字段名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否是忽略的字段
        /// </summary>
       
        public bool IsIgnoreField { get; set; }
    }
}
