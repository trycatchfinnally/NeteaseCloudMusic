using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeteaseCloudMusic.Wpf.BindingConverter
{
   public static class ValueConverterFactory
   {
         
       public static IValueConverter Boolean2VisibilityConverter { get; } = new Boolean2VisibilityConverter();
       public static IValueConverter Boolearn2SelectionModeConverter { get; } = new Boolearn2SelectionModeConverter();
       public static IValueConverter BoolOrEnum2Int32Converter { get; } = new BoolOrEnum2Int32Converter();
       public static IValueConverter Double2VisibilityConverter { get; } = new Double2VisibilityConverter();
       public static IValueConverter Enum2ConverterParameterConverter { get; } = new Enum2ConverterParameterConverter();
       public static IValueConverter HalfDoubleConverter { get; } = new HalfDoubleConverter();
       public static  IValueConverter Int2FormatStringConverter { get; } = new Int2FormatStringConverter();
       public static IValueConverter Text2PopupIsOpenConverter { get; } = new Text2PopupIsOpenConverter();
       public static IValueConverter TimeSpan2MillSecondsConverter { get; } = new TimeSpan2MillSecondsConverter();
     
   }

    public static class MultiValueConverterFactory
    {
        public static  IMultiValueConverter Coordinate2PointConverter { get; } = new Coordinate2PointConverter();
        public static IMultiValueConverter IndexOfCollectionConverter { get; } = new IndexOfCollectionConverter();
    //    public static IMultiValueConverter ItemBackgroundConverter { get; } = new ItemBackgroundConverter();
        public static IMultiValueConverter TimeSpan2TextConverter { get; } = new TimeSpan2TextConverter();
    }


}
