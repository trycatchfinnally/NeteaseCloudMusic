using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    /// <summary>
    /// 奇偶色listItem
    /// </summary>
    public class ItemBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic item = values[0];
            dynamic list = values[1];
            dynamic brushs = parameter;
            int index = list.IndexOf(item);
            return brushs[index%2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
