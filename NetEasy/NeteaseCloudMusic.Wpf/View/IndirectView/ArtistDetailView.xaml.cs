using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeteaseCloudMusic.Wpf.View.IndirectView
{
    /// <summary>
    /// ArtistView.xaml 的交互逻辑
    /// </summary>
    public partial class ArtistDetailView
    {
        public ArtistDetailView(ViewModel.IndirectView.ArtistDetailViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
            this.SetBinding(AlbumOrMvRefreshRequiredProperty, new Binding(nameof(ViewModel.IndirectView.ArtistDetailViewModel.AlbumOrMvOffsetCommand)));
        }
        /// <summary>
        /// 当专辑页面刷新到滚动到最后的时候
        /// </summary>
        private  ICommand AlbumOrMvRefreshRequired
        {
            get { return (ICommand)GetValue(AlbumOrMvRefreshRequiredProperty); }
            set { SetValue(AlbumOrMvRefreshRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlbumRefreshRequired.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty AlbumOrMvRefreshRequiredProperty =
            DependencyProperty.Register("AlbumOrMvRefreshRequired", typeof(ICommand), typeof(ScrollViewer), new PropertyMetadata(null));


        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var v1 = e.ExtentHeight - e.VerticalOffset;
            var v2 = e.ViewportHeight;
           
            if (v1 <= v2)
            {
                //currentPageoffset++;
                this.AlbumOrMvRefreshRequired?.Execute((e.Source as Control).Tag?.ToString());
            }
        }
    }
}
