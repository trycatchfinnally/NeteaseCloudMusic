using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
    /// <summary>
    /// 音乐的播放时间和总时长转换成00:00/04:30类型的文本
    /// </summary>
    public class TimeSpan2TextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                var temp1 = values[0].ToString().Split(':').Select(double.Parse).ToArray();
                var tempStr1 = (temp1[0] * 60 + temp1[1]).ToString("00") + ":" + temp1[2].ToString("00");
                if (values.Length == 2)
                {

                    var temp2 = values[1].ToString().Split(':').Select(double.Parse).ToArray();
                    var tempStr2 = (temp2[0] * 60 + temp2[1]).ToString("00") + ":" + temp2[2].ToString("00");
                    return $"{tempStr1}/{tempStr2}";
                }

                return tempStr1;
            }
            catch (FormatException)
            {
                if (values.Length == 1) return "00:00";
                return "00:00/00:00";

            }


        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
