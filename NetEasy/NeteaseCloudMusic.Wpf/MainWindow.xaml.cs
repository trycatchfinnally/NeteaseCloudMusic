using NeteaseCloudMusic.Wpf.ViewModel;
using System.Windows;
using System.Windows.Input;
using Unity.Attributes;
using System.ComponentModel;
using System.Xml.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Controls;


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
            get { return this.DataContext as MainWindowViewModel; }
            set { this.DataContext = value; }
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            const int durationMilliseconds = 800;
            this.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(durationMilliseconds))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 0.8 }
            });
            await Task.Delay(durationMilliseconds);
            this.Close();

        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnSize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
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
            root.Add(new XElement("WindowLeft", this.Left));
            root.Add(new XElement("WindowTop", this.Top));
            root.Add(new XElement("WindowSize", this.RenderSize));
            root.Add(new XElement("WindowStartupLocation", (int)this.WindowStartupLocation));
            root.Add(new XElement("WindowState", (int)this.WindowState));
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
    }
}
