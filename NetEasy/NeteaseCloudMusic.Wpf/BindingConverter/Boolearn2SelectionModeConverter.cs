using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    public class Boolearn2SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value ))
            {
                return SelectionMode.Extended;
            }

            return SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
