using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 加载和保存设置
    /// </summary>
    public static  class DataSettingManager
    {
        private const string fileName = "Settings.json";
        /// <summary>
        /// 加载设置
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataSetting LoadSetting(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            }
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<DataSetting>(json);
            }

            return new DataSetting();
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="dataSetting"></param>
        public static void SaveSetting(DataSetting dataSetting)
        {
            if (dataSetting == null)
            {
                throw new ArgumentNullException(nameof(dataSetting));
            }
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            var json = JsonConvert.SerializeObject(dataSetting);
            File.WriteAllText(filePath, json);
        }
    }
}
