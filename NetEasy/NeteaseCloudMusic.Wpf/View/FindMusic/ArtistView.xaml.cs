using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// ArtistView.xaml 的交互逻辑
    /// </summary>
    public partial class ArtistView
    {
        public ArtistView()
        {
            InitializeComponent();
            DataContextChanged += (sender, e) =>
            {
                if (e.NewValue != null)
                    SetBinding(NextPageCommandProperty, new Binding(nameof(ViewModel.ArtistViewModel.NextPageCommand)));
            };
        }
        private ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(ArtistView), new PropertyMetadata(null));

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var v1 = e.ExtentHeight - e.VerticalOffset;
            var v2 = e.ViewportHeight;

            if (v1 <= v2)
            {
                //currentPageoffset++;
                NextPageCommand?.Execute(null);
            }
        }
    }
}
