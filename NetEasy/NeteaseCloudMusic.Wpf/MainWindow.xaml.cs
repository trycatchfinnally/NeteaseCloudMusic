using NeteaseCloudMusic.Wpf.ViewModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using Unity.Attributes;


namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        [DependencyAttribute]
        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            const int durationMilliseconds = 800;
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(durationMilliseconds))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 0.8 }
            });
            await Task.Delay(durationMilliseconds);
            Close();

        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnSize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                btnSize_Click(null, null);


        }
        protected override void OnClosing(CancelEventArgs e)
        {
            var configXml = new XDocument();
            var root = new XElement("Root");
            root.Add(new XElement("WindowLeft", Left));
            root.Add(new XElement("WindowTop", Top));
            root.Add(new XElement("WindowSize", RenderSize));
            root.Add(new XElement("WindowStartupLocation", (int)WindowStartupLocation));
            root.Add(new XElement("WindowState", (int)WindowState));
            configXml.Add(root);
            var path = Path.Combine(Environment.CurrentDirectory, "WindowStartupInfo.xml");
            if (File.Exists(path))
                File.SetAttributes(path, FileAttributes.Normal);
            configXml.Save(path);
            File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.ReadOnly);
            base.OnClosing(e);
        }

        private async void BtnNavBack_Loaded(object sender, RoutedEventArgs e)
        {
           
            var button = e.Source as Button;

            while (true)
            {
                var navSer = CommonServiceLocator.ServiceLocator.Current.GetInstance<Prism.Regions.IRegionManager>();
                if (navSer != null && navSer.Regions.ContainsRegionWithName(Context.RegionName))
                {
                    navSer.Regions[Context.RegionName].NavigationService.Navigated += (xxx, xx) =>
                    {

                        button.Visibility = xx.NavigationContext.NavigationService.Journal.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
                    };
                    return;
                    // button.Visibility = navSer.Regions[Context.RegionName].NavigationService.Journal.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    button.Visibility = Visibility.Collapsed;
                }
                await Task.Delay(200);
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (tgSwitch.IsChecked==true)
            {
                this.leftPartPanel.Width = 140;
                this.bottomUserPart.Orientation = Orientation.Horizontal;
                hiddlePart.Visibility = Visibility.Visible;
            }
            else
            {
                this.leftPartPanel.Width = 40;
                this.bottomUserPart.Orientation = Orientation.Vertical;
                hiddlePart.Visibility = Visibility.Collapsed;

            }
        }
    }
}
