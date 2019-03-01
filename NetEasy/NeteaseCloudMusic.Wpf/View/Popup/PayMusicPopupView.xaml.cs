using Prism.Interactivity.InteractionRequest;
using System.Windows;

namespace NeteaseCloudMusic.Wpf.View.Popup
{
    /// <summary>
    /// PayMusicPopupView.xaml 的交互逻辑
    /// </summary>
    public partial class PayMusicPopupView
    {
        public PayMusicPopupView()
        {
            InitializeComponent();
         //   Loaded += PayMusicPopupView_Loaded;
        }
        private Confirmation Confirmation
        {
            get
            {
                return DataContext as Confirmation;
            }
        }
      
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (Confirmation != null)
            {
                Confirmation.Confirmed = false;
            }
            window?.Close();
        }

        private void BuyThisMusic_Click(object sender, RoutedEventArgs e)
        {
            if (Confirmation != null)
            {
                System.Diagnostics.Process.Start(
                    $"https://music.163.com/store/product/detail?id=5933052&songId={Confirmation.Content}");
                Confirmation.Confirmed = true;
            }

            Window.GetWindow(this)?.Close();
        }

        private void BtnOpenVip_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://music.163.com/store/vip?null");
            if (Confirmation != null)
            {
                Confirmation.Confirmed = true;
            }
            Window.GetWindow(this)?.Close();
        }
    }
}
