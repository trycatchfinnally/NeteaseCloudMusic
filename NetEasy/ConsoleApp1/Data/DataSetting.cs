using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 代表数据设置，连接信息等
    /// </summary>
    public class DataSetting
    {
        /// <summary>
        /// 数据提供者 OLEDB ODBC
        /// </summary>
        public string DataProvider { get; set; } = "sqlserver";
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string DataConnectionString { get; set; } = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename= E:\neteasy\NetEasy\NeteaseCloudMusic.Data\AppData\SqlServices.mdf;Integrated Security=True";
        /// <summary>
        /// 原始数据设置
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// 当前数据设置是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(DataProvider) && !string.IsNullOrEmpty(DataConnectionString);
        }
    }
}
