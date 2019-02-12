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
    /// 时间间隔与毫秒之间的转换
    /// </summary>
    public class TimeSpan2MillSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var temp = (TimeSpan)value;
                return temp.TotalMilliseconds;
            }

            throw new ArgumentException(nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds(System.Convert.ToDouble(value));
        }
    }
}
