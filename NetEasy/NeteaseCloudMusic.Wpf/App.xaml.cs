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
using NeteaseCloudMusic.Services.HttpCookie;
using NeteaseCloudMusic.Services.Identity;
using NeteaseCloudMusic.Wpf.Proxy;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;

namespace NeteaseCloudMusic.Wpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var inetworkService = Container.Resolve<INetWorkServices>();
            inetworkService.HttpExceptionAction = content => Container.Resolve<InteractionRequestsProxy>().AutoDisappearPopupRequest.Raise(new Notification { Title = "sdfg", Content = content });
            return Container.Resolve<MainWindow>();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            // var dentityService = CommonServiceLocator.ServiceLocator.Current.GetInstance<IdentityService>();
            SplashscreenWindow sw = new Wpf.SplashscreenWindow();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                var rect = (NeteaseCloudMusic.Wpf.Properties.Settings.Default.WindowLocation);
                sw.Left = rect.Left;
                sw.Top = rect.Top;
                sw.Width = rect.Width;
                sw.Height = rect.Height;
                sw.Show();
                base.OnStartup(e);
                if (MainWindow != null)
                {
                    MainWindow.Left = sw.Left;
                    MainWindow.Top = sw.Top;
                    MainWindow.Width = sw.Width;
                    MainWindow.Height = sw.Height;
                    MainWindow.Show();
                }
                const int durationMilliseconds = 800;
                sw.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(durationMilliseconds))
                {
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 0.8 }
                });
                await Task.WhenAll(Task.Delay(durationMilliseconds)
                    //, Session.RefreshLog()
                    );
                sw.Close();
            }
            catch
            {
                sw?.Close();
                base.OnStartup(e);
            }

        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.StackTrace);
            if (e.Exception is System.Net.Http.HttpRequestException)
            {
                e.Handled = true;
                return;
            }
            Application.Current.Shutdown();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICookieService, LocalFileCookieService>();
            containerRegistry.RegisterSingleton<INetWorkServices, NeteaseCloundMusicNetWorkServices>();
            containerRegistry.RegisterSingleton<IAudioPlayableServices, NAudioPlayableServices>();
            containerRegistry
                .RegisterSingleton<NeteaseCloudMusic.Services.LocalFile.IFileServices, Services.WindowsFileServices>();
            containerRegistry.RegisterSingleton<IdentityService, DefaultIdentityService>();
            containerRegistry.RegisterSingleton(typeof(InteractionRequestsProxy));
            containerRegistry.RegisterSingleton(typeof(PlayPartCore));




        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<MainRegionModule>();
        }

    }
}

