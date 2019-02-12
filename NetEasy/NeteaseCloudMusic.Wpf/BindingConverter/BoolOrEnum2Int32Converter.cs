using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    public class BoolOrEnum2Int32Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool || value is Enum)
            {
                return System.Convert.ToInt32(value);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (targetType == typeof(bool))
                return (int)value == 1;
            return Enum.ToObject(targetType, value);

        }
    }
}
