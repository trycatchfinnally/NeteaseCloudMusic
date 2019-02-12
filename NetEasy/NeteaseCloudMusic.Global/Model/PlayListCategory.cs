using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
    public class PlayListCategory
    {
        public string  Name { get; set; }
        /// <summary>
        /// 分类的名称
        /// </summary>
        public string  CategoryTypeName { get; set; }
        public bool  Hot { get; set; }
        public Int64 Id { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
