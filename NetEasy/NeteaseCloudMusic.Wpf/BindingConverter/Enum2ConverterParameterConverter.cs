using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    public class Enum2ConverterParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( parameter == null)
                return true;
            if (value is Enum)
                return Enum.Equals(value, Enum.Parse(value.GetType(), parameter.ToString()));
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            string parameterString = parameter?.ToString();
            if (parameterString == null)
                throw new ArgumentNullException(nameof(parameter));
             return Enum.Parse(targetType , parameterString);
        }
    }
}
