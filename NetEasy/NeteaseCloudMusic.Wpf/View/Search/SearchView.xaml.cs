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
using NeteaseCloudMusic.Wpf.ViewModel;

namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// SearchView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchView 
    {
        public SearchView(SearchViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.SetBinding(NextPageCommandProperty, new Binding(nameof(SearchViewModel.SearchResultNextPageCommand)));
            InitializeComponent();  
        }

        
        private ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(SearchView), new PropertyMetadata(null));
        private void TabSearchResult_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var temp = (bool)e.NewValue;
            if (temp )
            {
                dpSearchSuggest.Visibility = Visibility.Collapsed;
                suggestPopup.Opacity = 0;
            }
            else
            {
                dpSearchSuggest.Visibility = Visibility.Visible;
                suggestPopup.Opacity = 1;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var v1 = e.ExtentHeight - e.VerticalOffset;
            var v2 = e.ViewportHeight;
            var source = e.Source as FrameworkElement;
            if (v1 != 0 && v1 <= v2  )
            {
                //currentPageoffset++;
                NextPageCommand?.Execute(source.Tag);
            }
        }

       
    }
}
 
 
 
  