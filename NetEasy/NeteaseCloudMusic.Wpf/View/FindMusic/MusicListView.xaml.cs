using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// MusicListView.xaml 的交互逻辑
    /// </summary>
    public partial class MusicListView
    {
        public MusicListView()
        {

            InitializeComponent(); DataContextChanged += (sender, e) =>
            {
                if (e.NewValue != null)
                    SetBinding(NextPageCommandProperty, new Binding(nameof(ViewModel.MusicListViewModel.NextPageCommand)));
            };

        }




        private ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(MusicListView), new PropertyMetadata(null));




        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                this.txtSelectCategory.Text = e.AddedItems[0].ToString();
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = e.Source as Button;
            if (button != null)
            {
                this.txtSelectCategory.Text = button.Content.ToString();
            }
            var temp = this.lstAllCategories.Items?.Cast<object>().FirstOrDefault(x => x.ToString() == this.txtSelectCategory.Text);
            if (temp != null)
            {
                this.lstAllCategories.SelectedItem = temp;
            }
        }

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
