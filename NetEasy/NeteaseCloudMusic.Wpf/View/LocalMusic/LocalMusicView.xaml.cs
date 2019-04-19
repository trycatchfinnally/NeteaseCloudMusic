using NeteaseCloudMusic.Global.Model;
using NeteaseCloudMusic.Wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// LocalMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class LocalMusicView
    {
        #region 内部类
        private class GroupData
        {
            /// <summary>
            /// tab页下对应的第一个grid
            /// </summary>
            public Panel RootPanel { get; }
            /// <summary>
            /// tab对应的集合
            /// </summary>
            public System.Collections.ObjectModel.Collection<LocalMusic> BindingList { get; }
            /// <summary>
            /// 筛选的字段
            /// </summary>
            public Func<LocalMusic, string> GroupPropSelector { get; }
            public GroupData(Panel rootPanel, System.Collections.ObjectModel.Collection<LocalMusic> bindingList, Func<LocalMusic, string> groupProp)
            {
                RootPanel = rootPanel;
                BindingList = bindingList;
                GroupPropSelector = groupProp;
            }
        }
        public class SortByPinYinConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string text = value?.ToString().ToUpper();
                if (string.IsNullOrEmpty(text)) return '#';
                var msg = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(text[0]);
                if (msg == null)
                {
                    if (text[0] < 'A' || text[0] > 'Z')
                        return '#';
                    return text[0];
                }

                return msg[0].ToUpper()[0];
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        public LocalMusicView(LocalMusicViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
        private LocalMusicViewModel ViewModel
        {
            get { return DataContext as LocalMusicViewModel; }

        }
        [Obsolete("直接取名更好！", true)]
        private static Panel FindPanel(DependencyObject temp)
        {

            while (true)
            {
                if (temp == null) return null;
                var convertTemp = temp as Panel;
                // if (temp is Panel) return temp as Panel;
                if (convertTemp != null && convertTemp.Children.Count > 0 && convertTemp.Children[0] is ListBox)
                    return convertTemp;
                temp = VisualTreeHelper.GetParent(temp);
            }
        }
        /// <summary>
        /// 点击列表的分组按钮执行的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bntGroupKey_Click(object sender, RoutedEventArgs e)
        {

            Button btn = e.Source as Button;
            if (btn == null) return;
            // GroupData temp;
            Panel panel; string[] query;
            switch (btn.Tag.ToString().Trim())
            {
                case "1":
                    // temp = new GroupData(GdTab1Panel, ViewModel.MusicCollection, x => x.Title);
                    panel = this.GdTab1Panel;
                    query = ViewModel.MusicCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.Title, null, null, null))
                        .Select(x => x.Key.ToString()).ToArray();
                    break;
                case "2":
                    // temp = new GroupData(GdTab2Panel, ViewModel.ArtisCollection, x => x.ArtistsName.First());
                    panel = this.GdTab2Panel;
                    query = ViewModel.ArtistCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.Name, null, null, null))
                        .Select(x => x.Key.ToString()).ToArray();
                    break;
                case "3":
                    panel = this.GdTab3Panel;
                    query = ViewModel.MusicCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.AlbumName, null, null, null))
                        .Select(x => x.Key.ToString()).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("只有三个页面");
            }
            if (panel == null) throw new ArgumentException("未找到对应的容器对象");
            panel.Children[1].Visibility = Visibility.Visible;
            panel.Children[0].Visibility = Visibility.Hidden;
            // var query = temp.BindingList.GroupBy(x => SortHelper.SortByPinYinConverter.Convert(temp.GroupPropSelector(x), null, null, null)).Select(x => x.Key.ToString()).ToArray();
            Panel panel1 = panel.Children[1] as Panel;
            foreach (ContentControl item in panel1.Children)
                item.IsEnabled = query.Contains(item.Content.ToString());
            //panel.Children.Cast<Button>().Where(x => !query.Contains(x.Content.ToString())).Each(x => x.IsEnabled = false);


        }
        /// <summary>
        /// 点击排序面板的排序按钮触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGroupPanel_Click(object sender, RoutedEventArgs e)
        {

            Button btn = e.Source as Button;
            if (btn == null) return;
            //GroupData temp;
            //switch (btn.Tag.ToString().Trim())
            //{
            //    case "1":
            //        temp = new GroupData(GdTab1Panel, ViewModel.MusicCollection, x => x.Title);
            //        break;
            //    case "2":
            //        temp = new GroupData(GdTab2Panel, ViewModel.ArtisCollection, x => x.ArtistsName.First());
            //        break;
            //    case "3":
            //        temp = new GroupData(GdTab3Panel, ViewModel.AlbumCollection, x => x.AlbumName);
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException("只有三个页面");
            //}
            //temp.RootPanel.Children[1].Visibility = Visibility.Hidden;
            //temp.RootPanel.Children[0].Visibility = Visibility.Visible;
            //var query = temp.BindingList.GroupBy(x => SortHelper.SortByPinYinConverter.Convert(temp.GroupPropSelector(x), null, null, null)).ToDictionary(x => x.Key.ToString(), x => x.First());
            Panel panel; Dictionary<string, object> query;
            switch (btn.Tag.ToString().Trim())
            {
                case "1":
                    // temp = new GroupData(GdTab1Panel, ViewModel.MusicCollection, x => x.Title);
                    panel = this.GdTab1Panel;
                    query = ViewModel.MusicCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.Title, null, null, null))
                        .ToDictionary(x => x.Key.ToString(), x => (object)x.First());
                    break;
                case "2":
                    // temp = new GroupData(GdTab2Panel, ViewModel.ArtisCollection, x => x.ArtistsName.First());
                    panel = this.GdTab2Panel;
                    query = ViewModel.ArtistCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.Name, null, null, null))
                        .ToDictionary(x => x.Key.ToString(), x => (object)x.First());
                    break;
                case "3":
                    panel = this.GdTab3Panel;
                    query = ViewModel.MusicCollection.GroupBy(x =>
                            SortHelper.SortByPinYinConverter.Convert(x.AlbumName, null, null, null))
                        .ToDictionary(x => x.Key.ToString(), x => (object)x.First());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("只有三个页面");
            }
            panel.Children[1].Visibility = Visibility.Hidden;
            panel.Children[0].Visibility = Visibility.Visible;var LstLocalMusic = panel.Children[0] as ListBox;
            LstLocalMusic.ScrollIntoView(query[btn.Content.ToString()]);

        }
    }
    public static class SortHelper
    {
        public static IValueConverter SortByPinYinConverter { get; } = new LocalMusicView.SortByPinYinConverter();

    }
}
