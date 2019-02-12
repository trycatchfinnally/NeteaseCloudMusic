using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// 指定限制更新字段。 
    /// </summary>
    public class UpdateFields
    {
        private List<string> _fields;
        private Dictionary<string, string> _hashFields;
        private UpdateFieldsOptions _option;
        
        public List<string> Fields
        {
            get
            {
                return _fields;
            }

            
        }

        public UpdateFieldsOptions Option
        {
            get
            {
                return _option;
            }

            set
            {
                _option = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="option">指定字段选项</param>
        /// <param name="fields">指定字段集合</param>
        public UpdateFields(UpdateFieldsOptions option, params string[] fields)
        {
            this._option = option;
            this._fields = new List<string>();
            this._hashFields = new Dictionary<string, string>();
            string str = null;
            if (fields != null)
            {
                foreach (string str2 in fields)
                {
                    if (!string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(str2.Trim()))
                    {
                        str = str2.Trim();
                        if (!this._hashFields.ContainsKey(str.ToUpper()))
                        {
                            this._hashFields.Add(str.ToUpper(), str);
                            this._fields.Add(str);
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 是否包含指定的字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool ContainsField(string field)
        {
            return !string.IsNullOrWhiteSpace(field)&& this._hashFields.ContainsKey(field.ToUpper());

        }
    }
}
