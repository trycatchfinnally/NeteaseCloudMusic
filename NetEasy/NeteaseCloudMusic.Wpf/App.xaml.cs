using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using NeteaseCloudMusic.Wpf.Modules;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using NeteaseCloudMusic.Services.NetWork;
using NeteaseCloudMusic.Services.AudioDecode;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {

            return this.Container.Resolve<MainWindow>();
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
                if (this.MainWindow != null)
                {
                    this.MainWindow.Left = sw.Left;
                    this.MainWindow.Top = sw.Top;
                    this.MainWindow.Width = sw.Width;
                    this.MainWindow.Height = sw.Height;
                    this.MainWindow.WindowStartupLocation = sw.WindowStartupLocation;
                    this.MainWindow.WindowState = sw.WindowState;
                    this.MainWindow.Show();
                }
                //谈出效果退出
                const int durationMilliseconds = 800;
                sw.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(durationMilliseconds))
                {
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 0.8 }
                });
                await Task.Delay(durationMilliseconds);
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

