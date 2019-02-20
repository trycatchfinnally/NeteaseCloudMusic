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
            Loaded += PayMusicPopupView_Loaded;
        }
        private Confirmation Confirmation
        {
            get
            {
                return DataContext as Confirmation;
            }
        }
        private void PayMusicPopupView_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnOpenVip.Click -= BtnOpenVip_Click;
            this.btnOpenVip.Click += BtnOpenVip_Click;
            this.buyThisMusic.Click -= BuyThisMusic_Click;
            this.buyThisMusic.Click += BuyThisMusic_Click;
        }

        private void BuyThisMusic_Click(object sender, RoutedEventArgs e)
        {
            if (Confirmation != null)
                System.Diagnostics.Process.Start($"https://music.163.com/store/product/detail?id=5933052&songId={Confirmation.Content}");
        }

        private void BtnOpenVip_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://music.163.com/store/vip?null");
        }
    }
}
