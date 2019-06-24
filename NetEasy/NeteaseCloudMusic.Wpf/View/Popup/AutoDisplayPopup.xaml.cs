using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeteaseCloudMusic.Wpf.View.Popup
{
    /// <summary>
    /// AutoDisplayPopup.xaml 的交互逻辑
    /// </summary>
    public partial class AutoDisplayPopup
    {
        //private DoubleAnimation autoHiddleAnimation;
        public AutoDisplayPopup()
        {
            InitializeComponent();
        }


        private async void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            const int durationMillSeconds = 2500;
            // if (autoHiddleAnimation == null)
            var autoHiddleAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(durationMillSeconds),
                To = 0,
                FillBehavior = FillBehavior.Stop
            ,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            this.BeginAnimation(OpacityProperty, autoHiddleAnimation);

            await Task.Delay(durationMillSeconds);

            var window = Window.GetWindow(this);
            window.Visibility = Visibility.Hidden;
            //await Task.Delay(durationMillSeconds);
            window.Visibility = Visibility.Visible;
            this.Opacity = 1;
            window?.Close();
        }
    }
}
