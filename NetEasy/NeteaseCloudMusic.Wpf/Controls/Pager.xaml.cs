using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeteaseCloudMusic.Wpf.Controls
{
    /// <summary>
    /// Pager.xaml 的交互逻辑
    /// </summary>
    public partial class Pager : UserControl
    {
        /// <summary>
        /// 每一个按钮的高度，同时也是按钮的最小宽度
        /// </summary>
        private const double PageButtonHeight = 30;
        /// <summary>
        /// 每一个按钮与两边的距离
        /// </summary>
        private const double SideMargin = 5;
        #region  事件和属性
       
        /// <summary>
        /// 第i页的事件
        /// </summary>
        public static RoutedEvent PageEvent;
        public event RoutedEventHandler Page
        {
            add { AddHandler(PageEvent, value); }
            remove { RemoveHandler(PageEvent, value); }
        }


        /// <summary>
        /// 表示总的数目
        /// </summary>
        public uint Total
        {
            get { return (uint)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Total.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(uint), typeof(Pager), new PropertyMetadata(uint.MinValue, TotalChangedCallback, CoerceTotal));

        /// <summary>
        /// 当前页
        /// </summary>
        public uint CurrentPage
        {
            get { return (uint)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(uint), typeof(Pager), new PropertyMetadata(uint.MinValue, PageChanged));


        /// <summary>
        /// 页的数量
        /// </summary>
        public uint CountPerPage
        {
            get { return (uint)GetValue(CountPerPageProperty); }
            set { SetValue(CountPerPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountPerPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountPerPageProperty =
            DependencyProperty.Register("CountPerPage", typeof(uint), typeof(Pager), new PropertyMetadata((uint)30));

        /// <summary>
        /// 表示被选中页的颜色
        /// </summary>
        public Brush SelectedBorderBrush
        {
            get { return (Brush)GetValue(SelectedBorderBrushProperty); }
            set { SetValue(SelectedBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBorderBrushProperty =
            DependencyProperty.Register("SelectedBorderBrush", typeof(Brush), typeof(Pager), new PropertyMetadata(Brushes.Red));

         
        #endregion

        public Pager()
        {
            InitializeComponent();
        }

        static Pager()
        {
            PageEvent = EventManager.RegisterRoutedEvent(nameof(Page), RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(Pager));
        }
        /// <summary>
        /// 总的数量不小于当前页乘以每页的数量
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceTotal(DependencyObject d, object value)
        {
            var instance = d as Pager;
            if (instance == null)
            {
                throw new ArgumentException();
            }
            var currentValue = Convert.ToUInt32(value);
            var temp = instance.CountPerPage;
            if (currentValue < temp)
            {
                currentValue = temp;
            }

            return currentValue;
        }
        /// <summary>
        /// 当总的歌曲数量发生变化的时候执行的回调
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TotalChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Pager;
            if (instance == null)
            {
                throw new ArgumentException();
            }

            instance.CurrentPage = 1;
             
        }

        private static void UpdatePageButton(Pager instance, uint total, uint nav2Page = 1)
        {
            if (instance.CountPerPage == 0)
            {
                throw new ArgumentException();
            }
            instance.prevButton.IsEnabled = false;
            instance.nextButton.IsEnabled = false;
            instance.pagePanel.Children.Clear();
            
            var totalPage = Convert.ToUInt32(Math.Ceiling(total / (double)instance.CountPerPage));
            //中间可用部分=总可用部分-两边上下页及其边距占据部分
            var aviablePageWidth = instance.ActualWidth - 2 * (PageButtonHeight + SideMargin * 2);
            //当画面还没有呈现的时候
            if (aviablePageWidth < 0) return;
            var pageButtonWidth = PageButtonHeight;
            if (nav2Page>=1000)
            {
                pageButtonWidth = 50;
            }
            else if(nav2Page>=100)
            {
                pageButtonWidth = 40;
            }
            //instance.pagePanel.Measure(new Size(aviablePageWidth,PageButtonHeight));
            //中间最大可以放置的按钮数量=中间可用部分/每个按钮的宽度及其边距，结果向下取整
            var visiableButtonCount = Convert.ToUInt32(Math.Floor(aviablePageWidth / (pageButtonWidth + SideMargin * 2)));
            switch (totalPage)
            {
                case 0:
                    break;
                case 1:
                    instance.pagePanel.Children.Add(new Button() { Content = 1 });
                    break;
                default:
                    if (visiableButtonCount >= totalPage)
                    {
                        for (int i = 0; i < totalPage; i++)
                        {
                            instance.pagePanel.Children.Add(new Button() { Content = 1 + i });

                        }
                    }
                    else
                    {
                        instance.pagePanel.Children.Add(new Button() { Content = 1 });

                        if (nav2Page <= 2)
                        {

                            for (uint i = 1; i < visiableButtonCount - 1; i++)
                            {
                                instance.pagePanel.Children.Add(new Button() { Content = 1 + i });
                            }

                            instance.pagePanel.Children.Add(new TextBlock() { Text = "...", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                            instance.pagePanel.Children.Add(new Button() { Content = totalPage });
                        }
                        else
                        {
                            instance.pagePanel.Children.Add(new TextBlock() { Text = "...", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                            if (visiableButtonCount == 3)
                            {
                                instance.pagePanel.Children.Add(new Button() { Content = nav2Page });
                                break;
                            }

                            uint minPage = nav2Page;
                            uint maxPage = visiableButtonCount - 4 + nav2Page;
                            if (visiableButtonCount - 3 + nav2Page >= totalPage)
                            {
                                minPage = totalPage - visiableButtonCount + 3;
                                maxPage = totalPage ;
                                //if (visiableButtonCount - 4 + nav2Page - totalPage<=1)
                                //{
                                //    minPage--;
                                //    maxPage = totalPage;
                                //}
                            }

                            for (uint i = minPage; i <= maxPage; i++)
                            {
                                instance.pagePanel.Children.Add(new Button() { Content = i });

                            }
                            if (totalPage != maxPage)
                            {
                                if (totalPage != 4 )
                                    instance.pagePanel.Children.Add(new TextBlock() { Text = "...", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center });
                                instance.pagePanel.Children.Add(new Button() { Content = totalPage });
                            }
                        }
                    }
                    break;
            }

            var children = instance.pagePanel.Children;
            foreach (var child in children)
            {
                if (child is ButtonBase bb)
                {
                    if (Convert.ToUInt32(bb.Content) == nav2Page)
                    {
                        bb.BorderBrush = instance.SelectedBorderBrush;
                    }
                    bb.Click += Bb_Click;
                }
            }
            instance.prevButton.IsEnabled = instance.CurrentPage > 1;
            instance.nextButton.IsEnabled = instance.CurrentPage != totalPage;


            
        }

        private static void Bb_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as ButtonBase;
            if (btn == null) throw new ArgumentException();
            Pager instance = null;
            DependencyObject dObject = btn;
            while (true)
            {
                if (instance != null)
                {
                    break;
                }

                if (dObject == null)
                {
                    throw new ArgumentException("未找到");
                }
                dObject = VisualTreeHelper.GetParent(dObject);
                instance = dObject as Pager;

            }

            if (btn.BorderBrush==instance.SelectedBorderBrush)
            {
                 return;
                 
            }
            instance.CurrentPage = Convert.ToUInt32(btn.Content);
            instance.RaiseEvent(new RoutedEventArgs(PageEvent, instance));

        }

        /// <summary>
        /// 当选中的页发生变化的时候执行的逻辑。渲染重新绘制
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void PageChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as Pager;
            if (instance == null) throw new ArgumentException();
            UpdatePageButton(instance, instance.Total, instance.CurrentPage);
        }
        private void PrevButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            UpdatePageButton(this, Total, CurrentPage);
        }
    }
}
