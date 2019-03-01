using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.View.Popup
{
    /// <summary>
    /// AddPlayListPopupView.xaml 的交互逻辑
    /// </summary>
    public partial class AddPlayListPopupView
    {
        public AddPlayListPopupView()
        {
            InitializeComponent();
        }

        private Confirmation Confirmation
        {
            get
            {
                return DataContext as Confirmation;
            }
        }
        private void TxtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtTitle.Text))
            {
                this.txtNotEmpty.Visibility = Visibility.Visible;
                this.btnComfirm.IsEnabled = false;
            }
            else
            {
                this.txtNotEmpty.Visibility = Visibility.Hidden;
                this.btnComfirm.IsEnabled = true;
            }
        }

        private void BtnComfirm_Click(object sender, RoutedEventArgs e)
        {
            if (Confirmation != null)
            {
                Confirmation.Confirmed = true;
                Confirmation.Content = txtTitle.Text;
            }
            Window.GetWindow(this).Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Confirmation != null)
            {
                Confirmation.Confirmed = false;
            }
            Window.GetWindow(this).Close();
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtTitle.Text = string.Empty;
        }
    }
}
