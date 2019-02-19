using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Wpf.View.Popup
{
    /// <summary>
    /// LoginPopupView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPopupView : ContentControl
    {
        public LoginPopupView()
        {
            InitializeComponent();
            Loaded += LoginPopupView_Loaded;
             
        }
        private Confirmation Confirmation
        {
            get
            {
                return DataContext as Confirmation;
            }
        }
        private void LoginPopupView_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnClose.Click -= BtnClose_Click;
            this.txtPhone.TextChanged -= TxtPhone_TextChanged;
            this.txtPassword.PasswordChanged -= TxtPassword_PasswordChanged;
            this.btnLogin.Click -= BtnLogin_Click;
            this.txtPhone.TextChanged += TxtPhone_TextChanged;
            this.txtPassword.PasswordChanged += TxtPassword_PasswordChanged;
            this.btnLogin.Click += BtnLogin_Click;
            this.btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (Confirmation != null)
                Confirmation.Confirmed = false;
            window?.Close();
        }


        private void TxtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnLogin.IsEnabled = !(string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.txtPassword.Password));
            this.txtError.Text = string.Empty;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.btnLogin.IsEnabled = !(string.IsNullOrEmpty(this.txtPhone.Text) || string.IsNullOrEmpty(this.txtPassword.Password));
            this.txtError.Text = string.Empty;

        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtPhone.Text.Length != 11)
            { this.txtError.Text = "请输入11位数字的电话号码"; return; }
            if (!string.IsNullOrEmpty(this.txtError.Text)) return;
            var loginResult = await Session.LogInByCellPhone(this.txtPhone.Text, this.txtPassword.Password);
            if (loginResult != "OK")
            {
                this.txtError.Text = loginResult;
                if (Confirmation != null)
                    Confirmation.Confirmed = false;
            }
            else
            {
                if (Confirmation != null)
                    Confirmation.Confirmed = true;
                Window.GetWindow(this).Close();
            }
        }
    }
}
