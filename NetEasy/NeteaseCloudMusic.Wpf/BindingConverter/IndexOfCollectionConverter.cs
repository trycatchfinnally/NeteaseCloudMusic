using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    /// <summary>
    /// 获取集合中元素在集合中的索引
    /// </summary>
    public class IndexOfCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var item = values[0];
            var  collection = values[1] as System.Collections.IList;
            if(collection!=null )
            return (collection.IndexOf(item) + 1).ToString("00");
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
