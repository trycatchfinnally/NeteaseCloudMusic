using System;
using System.Globalization;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    public class TextLenthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var txt = value as System.Windows.Controls.TextBox;
            if (txt != null)
            {
                return txt.MaxLength - (txt.Text?.Length ?? 0);
            }
            return 1024;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
