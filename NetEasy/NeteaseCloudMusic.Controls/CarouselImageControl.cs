using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace NeteaseCloudMusic.Controls
{
    /// <summary>
    /// 表示轮播图控件,参考
    /// <see>
    ///         <cref>https://github.com/NaBian/HandyControl</cref>
    ///     </see>
    /// </summary>
    [TemplatePart(Name = ElementPanelPage, Type = typeof(Panel))]
    [TemplatePart(Name = ElementItemsControl, Type = typeof(Panel))]
    [DefaultProperty("Items")]
    [ContentProperty("Items")]
    public class CarouselImageControl : ListBox
    {
        /// <summary>
        /// 下面的滑块名
        /// </summary>
        private const string ElementPanelPage = "PART_PanelPage";
        /// <summary>
        /// 项目控件名
        /// </summary>
        private const string ElementItemsControl = "PART_ItemsControl";
        private Panel _panelPage;

        private bool _appliedTemplate;

        private Panel _itemsControl;

        private int _pageIndex = -1;

        private Button _selectedButton;

        private DispatcherTimer _updateTimer;

        private readonly List<double> _widthList = new List<double>();

        static CarouselImageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CarouselImageControl),
                new FrameworkPropertyMetadata(typeof(CarouselImageControl)));
        }

        public CarouselImageControl()
        {
            CommandBindings.Add(new CommandBinding(this.PrevCommand??(PrevCommand=new RoutedCommand()),
                (sender,e)=>PageIndex--));
            CommandBindings.Add(new CommandBinding(this.NextCommand ?? (NextCommand = new RoutedCommand()), (sender, e) => PageIndex++));
            this.Loaded += (sender, e) => UpdatePageButtons();
        }
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (this.Items.Count == 0) return;
                if (value == _pageIndex) return;
                if (value < 0)
                    _pageIndex = Items.Count - 1;
                else if (value >= Items.Count)
                    _pageIndex = 0;
                else _pageIndex = value;
                UpdatePageButtons(_pageIndex);

            }
        }
        /// <summary>
        /// 是否自动切换
        /// </summary>
        public bool AutoRun
        {
            get { return (bool)GetValue(AutoRunProperty); }
            set { SetValue(AutoRunProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoRun.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoRunProperty =
            DependencyProperty.Register("AutoRun", typeof(bool), typeof(CarouselImageControl), new PropertyMetadata(true,
                (sender, e) =>
                {
                    var temp = (CarouselImageControl)sender;
                    temp.SwitchTimer(Convert.ToBoolean(e.NewValue));
                }));


        /// <summary>
        /// 自动切换的时间间隔
        /// </summary>
        public TimeSpan Interval
        {
            get { return (TimeSpan)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(CarouselImageControl), new PropertyMetadata(TimeSpan.FromSeconds(2)));




        internal  ICommand PrevCommand
        {
            get { return (ICommand)GetValue(PrevCommandProperty); }
            set { SetValue(PrevCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrevCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrevCommandProperty =
            DependencyProperty.Register("PrevCommand", typeof(ICommand), typeof(CarouselImageControl), new PropertyMetadata(null));




        internal ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextCommand.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(CarouselImageControl), new PropertyMetadata(null));




        public override void OnApplyTemplate()
        {
            this._appliedTemplate = false;
            this._panelPage?.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(ButtonPages_Click));
            base.OnApplyTemplate();
            this._itemsControl = GetTemplateChild(ElementItemsControl) as StackPanel;

            this._panelPage = GetTemplateChild(ElementPanelPage) as Panel;

            if (_panelPage == null || _itemsControl == null) throw new ArgumentNullException();
            this._panelPage.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonPages_Click));
            this._appliedTemplate = true;
            SwitchTimer(AutoRun);
            UpdatePageButtons(this._pageIndex);


        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdatePageButtons();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateItemsPosition();
        }
        /// <summary>
        /// 底部的切换页按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPages_Click(object sender, RoutedEventArgs e)
        {
            if (this._selectedButton != null && _selectedButton.Content is Border)
            {
                ((Border)_selectedButton.Content).Background = Brushes.White;
            }

            _selectedButton = e.OriginalSource as Button;
            if (this._selectedButton != null && _selectedButton.Content is Border)
            {
                ((Border)_selectedButton.Content).Background = Brushes.Red;
            }
            var index = _panelPage.Children.IndexOf(_selectedButton);
            if (index != -1)
                PageIndex = index;
        }

        private void SwitchTimer(bool runOrNot)
        {
            if (!this._appliedTemplate) return;
            if (this._updateTimer != null)
            {
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer.Stop();
                _updateTimer = null;
            }

            if (!runOrNot) return;
            _updateTimer = new DispatcherTimer { Interval = Interval };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (IsMouseOver) return;
            PageIndex++;
        }
        /// <summary>
        /// 计算位移的宽度并且更新页按钮
        /// </summary>
        /// <param name="index"></param>
        private void UpdatePageButtons(int index = -1)
        {
            if (!this._appliedTemplate) return;
            var count = Items.Count;
            _widthList.Clear();
            _widthList.Add(0);
            var width = 0d;
            foreach (FrameworkElement item in this._itemsControl.Children)
            {
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                width += this.ActualWidth;
                _widthList.Add(width);
            }
            _panelPage.Children.Clear();
            for (var i = 0; i < count; i++)
            {
                _panelPage.Children.Add(CreatePateButton());
            }
            if (index == -1)
            {
                if (count > 0)
                {
                    var button = _panelPage.Children[0];
                    button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, button));
                }
            }
            else if (index >= 0 && index < count)
            {
                var button = _panelPage.Children[index];
                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, button));
                UpdateItemsPosition();
            }
        }
        /// <summary>
        /// 更新项的位置
        /// </summary>
        private void UpdateItemsPosition()
        {
            if (!_appliedTemplate) return;
            if (Items.Count == 0) return;
            this._itemsControl.BeginAnimation(MarginProperty, new ThicknessAnimation(
                new Thickness(-_widthList[PageIndex], 0, 0, 0), new Duration(TimeSpan.FromMilliseconds(200)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            }
            );

        }


       

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <returns></returns>
        private UIElement CreatePateButton()
        {
            return new Button
            {
                Style = this._panelPage.TryFindResource("PageButtonStyle") as Style,
                Content = new Border
                {
                    Width = 10,
                    Height = 10,
                    CornerRadius = new CornerRadius(5),
                    Background = Brushes.White,
                    Margin = new Thickness(5, 0, 5, 0),
                    BorderThickness = new Thickness(1),
                   
                }
            };
        }
      
    }
}
