using NeteaseCloudMusic.Services.AudioDecode;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Wpf.Modules;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override   Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "WindowStartupInfo.xml");
            SplashscreenWindow sw = new Wpf.SplashscreenWindow();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                XDocument xdoc = null;
                if (File.Exists(path))
                {
                    xdoc = XDocument.Load(path);
                }
                if (xdoc != null)
                {
                    var root = xdoc.Root;
                    if (root == null)
                        throw new ArgumentNullException();

                    sw.Left = Convert.ToDouble(root.Element("WindowLeft").Value);
                    sw.Top = Convert.ToDouble(root.Element("WindowTop").Value);
                    var temp = Size.Parse(root.Element("WindowSize").Value);
                    sw.Width = temp.Width;
                    sw.Height = temp.Height;
                    sw.WindowStartupLocation = (WindowStartupLocation)int.Parse((root.Element("WindowStartupLocation").Value));
                    sw.WindowState = (WindowState)int.Parse(root.Element("WindowState").Value);
                }
                sw.Show();
                base.OnStartup(e);
                if (MainWindow != null)
                {
                    MainWindow.Left = sw.Left;
                    MainWindow.Top = sw.Top;
                    MainWindow.Width = sw.Width;
                    MainWindow.Height = sw.Height;
                    MainWindow.WindowStartupLocation = sw.WindowStartupLocation;
                    MainWindow.WindowState = sw.WindowState;
                    MainWindow.Show();
                }
                const int durationMilliseconds = 800;
                sw.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(durationMilliseconds))
                {
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 0.8 }
                });
                await Task.WhenAll(Task.Delay(durationMilliseconds), Session.RefreshLog());
                sw.Close();
            }
            catch
            {
                if (File.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }

                sw?.Close();
                base.OnStartup(e);
            }

        }
        /// <summary>
        /// 当出了异常才调用父类的show方法
        /// </summary>
        protected override void OnInitialized()
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "WindowStartupInfo.xml")))
                base.OnInitialized();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<INetWorkServices, NeteaseCloundMusicNetWorkServices>();
            containerRegistry.RegisterSingleton<IAudioPlayableServices, NAudioPlayableServices>();

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<MainRegionModule>();
        }

    }
}

