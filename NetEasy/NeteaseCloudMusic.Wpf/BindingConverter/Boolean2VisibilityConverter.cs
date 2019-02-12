using System;
using System.Globalization;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    /// <summary>
    /// bool和Visibility之间的转换，用来决定是否显示某一项的值
    /// </summary>.
   [ValueConversion(typeof(Boolean),typeof(System.Windows.Visibility))]
    public class Boolean2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var result = System.Windows.Visibility.Collapsed;
            if (value is bool  && System.Convert.ToBoolean(value))
                result = System.Windows.Visibility.Visible;

            if (parameter != null)
                // result =result == System.Windows.Visibility.Visible ? System.Windows.Visibility.Collapsed:System.Windows.Visibility.Visible;
                result = (System.Windows.Visibility)Math.Abs((int)result - 2);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
