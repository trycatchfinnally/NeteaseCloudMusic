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

namespace NeteaseCloudMusic.Wpf.Controls
{
    /// <summary>
    /// MenuPartControl.xaml 的交互逻辑
    /// </summary>
    public class MenuPartControl : UserControl
    {

        /// <summary>
        /// 获取或设置当前的状态是否为打开
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MenuPartControl), new PropertyMetadata(false));



        /// <summary>
        /// 点击用户头像触发的命令
        /// </summary>
        public ICommand UserImageCommand
        {
            get { return (ICommand)GetValue(UserImageCommandProperty); }
            set { SetValue(UserImageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserImageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserImageCommandProperty =
            DependencyProperty.Register("UserImageCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));





        /// <summary>
        /// 点击邮件信息触发的命令
        /// </summary>
        public ICommand EmailCommand
        {
            get { return (ICommand)GetValue(EmailCommandProperty); }
            set { SetValue(EmailCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EmailCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmailCommandProperty =
            DependencyProperty.Register("EmailCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));





        /// <summary>
        /// 点击设置按钮触发的命令
        /// </summary>
        public ICommand SettingCommand
        {
            get { return (ICommand)GetValue(SettingCommandProperty); }
            set { SetValue(SettingCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingCommandProperty =
            DependencyProperty.Register("SettingCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));





        /// <summary>
        /// 绑定到第一个列表选中发生变化的命令
        /// </summary>
        public ICommand SysListCommand
        {
            get { return (ICommand)GetValue(SysListCommandProperty); }
            set { SetValue(SysListCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SysListCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SysListCommandProperty =
            DependencyProperty.Register("SysListCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));



        /// <summary>
        /// 点击‘收藏的歌单’旁边的‘+’命令
        /// </summary>

        public ICommand AddTrackCommand
        {
            get { return (ICommand)GetValue(AddTrackCommandProperty); }
            set { SetValue(AddTrackCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddTrackCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddTrackCommandProperty =
            DependencyProperty.Register("AddTrackCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));

        /// <summary>
        /// 点击我的音乐部分触发的命令
        /// </summary>

        public ICommand MyMusicCommand
        {
            get { return (ICommand)GetValue(MyMusicCommandProperty); }
            set { SetValue(MyMusicCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyMusicCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyMusicCommandProperty =
            DependencyProperty.Register("MyMusicCommand", typeof(ICommand), typeof(MenuPartControl), new PropertyMetadata(null));


        /// <summary>
        /// 表示已创建的歌单
        /// </summary>
        public System.Collections.IEnumerable CreatedTracks
        {
            get { return (System.Collections.IEnumerable)GetValue(CreatedTracksProperty); }
            set { SetValue(CreatedTracksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CreatedTracks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CreatedTracksProperty =
            DependencyProperty.Register("CreatedTracks", typeof(System.Collections.IEnumerable), typeof(MenuPartControl), new PropertyMetadata(null));








        public System.Collections.IEnumerable FavoriteTracks
        {
            get { return (System.Collections.IEnumerable)GetValue(FavoriteTracksProperty); }
            set { SetValue(FavoriteTracksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FavoriteTracks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FavoriteTracksProperty =
            DependencyProperty.Register("FavoriteTracks", typeof(System.Collections.IEnumerable), typeof(MenuPartControl), new PropertyMetadata(null));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var listboxs = new[]
            {
                GetTemplateChild("PART_LstSysList"),GetTemplateChild("PART_LstLocal"),
                GetTemplateChild("PART_LstCreatedTracks"),GetTemplateChild("PART_LstFavoriteTracks")
            }.Cast<ListBox>().ToArray();

            foreach (var item in listboxs)
            {
                item.SelectionChanged += (sender, e) =>
                 {
                     var lstObj = sender as ListBox;
                     if (lstObj != null && lstObj.SelectedIndex != -1)
                     {
                         var temp = listboxs.Where(x => !ReferenceEquals(x, lstObj));
                         //.Each(x => x.SelectedIndex = -1);
                         foreach (var x in temp)
                         {
                             x.SelectedIndex = -1;
                         }
                     }
                 };

                //使滚轮能够作用到listbox
                item.PreviewMouseWheel += (sender, e) =>
                {


                    (sender as UIElement)?.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = sender
                    });

                };
            }
        }

    }


    public class CreatedListItemContainerStyleSelector:StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style  LikeStyle { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var itemscontrol = GetItemsControl(container);
            var index = itemscontrol.Items.IndexOf(item);
            if (index == 0)
                return LikeStyle;
            return DefaultStyle;
        }
        private ItemsControl GetItemsControl(DependencyObject container)
        {
            while (true)
            {
                if (container == null) return null;
                var temp = VisualTreeHelper.GetParent(container);
                if (temp == null) return null;
                if (temp is ItemsControl) return temp as ItemsControl;
                container = temp;
            }
        }
    }
}
