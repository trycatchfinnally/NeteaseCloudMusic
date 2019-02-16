using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// NewMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class NewMusicView
    {
        public NewMusicView()
        {
            InitializeComponent(); DataContextChanged += (sender, e) =>
            {
                if (e.NewValue != null)
                    SetBinding(NextPageCommandProperty, new Binding(nameof(ViewModel.NewMusicViewModel.NextPageCommand)));
            };
        }
        private ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(NewMusicView), new PropertyMetadata(null));

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var v1 = e.ExtentHeight - e.VerticalOffset;
            var v2 = e.ViewportHeight;

            if (v1 != 0 && v1 <= v2 && this.rdTopMusic.IsChecked != true)
            {
                //currentPageoffset++;
                NextPageCommand?.Execute(null);
            }
        }
    }
}
