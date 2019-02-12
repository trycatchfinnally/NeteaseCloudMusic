using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NeteaseCloudMusic.Wpf.Behaviors
{
    /// <summary>
    /// 自动滚动到选中的项的行为
    /// </summary>
    public   class AutoScrollBehavior: Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += ListBox_SelectionChanged;
        }

     
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lstBox = sender as ListBox;
            if (lstBox?.SelectedItem != null )
            {
                lstBox.Dispatcher.Invoke(()=>
                {
                    lstBox.UpdateLayout();
                    lstBox.ScrollIntoView(lstBox.SelectedItem);
                });
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= ListBox_SelectionChanged;
        }
    }
}
