using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Controls
{
  public   class ImageButton:Button
    {



        public ImageSource MusicImage
        {
            get { return (ImageSource)GetValue(MusicImageProperty); }
            set { SetValue(MusicImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MusicImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MusicImageProperty =
            DependencyProperty.Register("MusicImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));



    }
}
