using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
     
    /// <summary>
    /// 从点的X和Y值返回对应的Point
    /// </summary>
    public class Coordinate2PointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Point((double)values[0], (double)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            dynamic temp = value;
            return new[] {temp.X,temp.Y };
        }
    }
}
